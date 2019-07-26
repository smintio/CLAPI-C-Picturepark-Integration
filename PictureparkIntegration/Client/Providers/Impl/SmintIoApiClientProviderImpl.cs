using Client.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using SmintIo.CLAPI.Consumer.Client.Generated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Client.Contracts;
using System.Net;

namespace Client.Providers.Impl
{
    public sealed class SmintIoApiClientProviderImpl: IDisposable, ISmintIoApiClientProvider
    {
        private const int MaxRetryAttempts = 5;

        private readonly SmintIoAppOptions _options;

        private readonly IAuthDataProvider _authDataProvider;
        private readonly IAuthenticator _authenticator;

        private readonly HttpClient _http;

        private readonly AsyncRetryPolicy _retryPolicy;

        private bool _disposed;

        private readonly ILogger _logger;

        private readonly CLAPICOpenApiClient _clapicOpenApiClient;

        public SmintIoApiClientProviderImpl(
            IOptionsMonitor<SmintIoAppOptions> optionsAccessor,
            ILogger<SmintIoApiClientProviderImpl> logger,
            IAuthenticator authenticator,            
            IAuthDataProvider authDataProvider)
        {
            _options = optionsAccessor.CurrentValue;
            
            _authenticator = authenticator;
            
            _authDataProvider = authDataProvider;

            _disposed = false;

            _http = new HttpClient();

            _logger = logger;

            _retryPolicy = GetRetryStrategy();

            _clapicOpenApiClient = new CLAPICOpenApiClient(_http);

            _clapicOpenApiClient.BaseUrl = $"https://{_options.TenantId}-clapi.smint.io/consumer/v1";
        }

        public async Task<SmintIoGenericMetadata> GetGenericMetadataAsync()
        {
            _logger.LogInformation("Receiving generic metadata from Smint.io...");

            _clapicOpenApiClient.AccessToken = _authDataProvider.SmintIo.AccessToken;

            var syncGenericMetadata = await _retryPolicy.ExecuteAsync(async () =>
                await _clapicOpenApiClient.GetGenericMetadataForSyncAsync());

            var smintIoGenericMetadata = new SmintIoGenericMetadata();

            smintIoGenericMetadata.ContentProviders = GetGroupedMetadataElementsForImportLanguages(syncGenericMetadata.Providers);
            smintIoGenericMetadata.ContentCategories = GetGroupedMetadataElementsForImportLanguages(syncGenericMetadata.Content_categories);

            smintIoGenericMetadata.LicenseTypes = GetGroupedMetadataElementsForImportLanguages(syncGenericMetadata.License_types);
            smintIoGenericMetadata.ReleaseStates = GetGroupedMetadataElementsForImportLanguages(syncGenericMetadata.Release_states);

            smintIoGenericMetadata.LicenseUsages = GetGroupedMetadataElementsForImportLanguages(syncGenericMetadata.License_usages);
            smintIoGenericMetadata.LicenseSizes = GetGroupedMetadataElementsForImportLanguages(syncGenericMetadata.License_sizes);
            smintIoGenericMetadata.LicensePlacements = GetGroupedMetadataElementsForImportLanguages(syncGenericMetadata.License_placements);
            smintIoGenericMetadata.LicenseDistributions = GetGroupedMetadataElementsForImportLanguages(syncGenericMetadata.License_distributions);
            smintIoGenericMetadata.LicenseGeographies = GetGroupedMetadataElementsForImportLanguages(syncGenericMetadata.License_geographies);
            smintIoGenericMetadata.LicenseVerticals = GetGroupedMetadataElementsForImportLanguages(syncGenericMetadata.License_verticals);

            _logger.LogInformation("Received generic metadata from Smint.io");

            return smintIoGenericMetadata;
        }

        private IList<SmintIoMetadataElement> GetGroupedMetadataElementsForImportLanguages(LocalizedMetadataElements localizedMetadataElements)
        {
            return localizedMetadataElements
                .Where(localizedMetadataElement => _options.ImportLanguages.Contains(localizedMetadataElement.Culture))
                .GroupBy(localizedMetadataElement => localizedMetadataElement.Metadata_element.Key)
                .Select((group) =>
                {
                    return new SmintIoMetadataElement()
                    {
                        Key = group.Key,
                        Values = group.ToDictionary(metadataElement => metadataElement.Culture, metadataElement => metadataElement.Metadata_element.Name)
                    };
                })
                .ToList();
        }

        public async Task<(IList<SmintIoAsset>, string)> GetAssetsAsync(string continuationUuid)
        {
            _logger.LogInformation("Receiving assets from Smint.io...");

            IList<SmintIoAsset> result;

            (result, continuationUuid) = await LoadAssetsAsync(continuationUuid);
            
            _logger.LogInformation($"Received {result.Count()} assets from Smint.io");

            return (result, continuationUuid);
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        private AsyncRetryPolicy GetRetryStrategy()
        {
            return Policy
                .Handle<ApiException>()
                .Or<Exception>()
                .WaitAndRetryAsync(
                    MaxRetryAttempts,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    async (ex, timespan, context) =>
                    {
                        _logger.LogError(ex, "Error communicating to Smint.io");

                        if (ex is ApiException apiEx)
                        {
                            if (apiEx.StatusCode == (int)HttpStatusCode.Forbidden || apiEx.StatusCode == (int)HttpStatusCode.Unauthorized)
                            {
                                var result = await _authenticator.RefreshSmintIoTokenAsync(_authDataProvider.SmintIo.RefreshToken);

                                _authDataProvider.SmintIo.AccessToken = result.AccessToken;

                                // backoff and try again 

                                return;
                            }
                            else if (apiEx.StatusCode == (int)HttpStatusCode.TooManyRequests)
                            {
                                // backoff and try again

                                return;
                            }

                            // expected error happened server side, most likely our problem, cancel

                            throw ex;
                        }

                        // some server side or communication issue, backoff and try again
                    });
        }

        private async Task<(IList<SmintIoAsset>, string)> LoadAssetsAsync(string continuationUuid)
        {
            _clapicOpenApiClient.AccessToken = _authDataProvider.SmintIo.AccessToken;

            IList<SmintIoAsset> assets = new List<SmintIoAsset>();

            SyncLicensePurchaseTransactionQueryResult syncLptQueryResult = await _retryPolicy.ExecuteAsync(async () =>
            {
                return await _clapicOpenApiClient.GetLicensePurchaseTransactionsForSyncAsync(
                    continuationUuid: continuationUuid,
                    limit: 10);
            });

            if (syncLptQueryResult.Count == 0)
            {
                return (assets, syncLptQueryResult.Continuation_uuid);
            }

            foreach (var lpt in syncLptQueryResult.License_purchase_transactions)
            {
                bool? isEditorialUse = null;

                foreach (var license_usage_constraint in lpt.License_usage_constraints)
                {
                    // make sure we do not store editorial use information if no information is there!

                    if (license_usage_constraint.Effective_is_editorial_use != null)
                    {
                        if (license_usage_constraint.Effective_is_editorial_use == true)
                        {
                            // if we have a restrictions, always indicate

                            isEditorialUse = true;
                        }
                        else if (license_usage_constraint.Effective_is_editorial_use == false)
                        {
                            // if we have no restriction, only store, if we have no other restriction

                            if (isEditorialUse == null)
                                isEditorialUse = false;
                        }
                    }
                }

                string url = $"https://{_options.TenantId}.smint.io/project/{lpt.Project_uuid}/content-element/{lpt.Content_element.Uuid}";

                var asset = new SmintIoAsset()
                {
                    LPTUuid = lpt.Uuid,
                    CPTUuid = lpt.Cart_purchase_transaction_uuid,
                    State = lpt.State,
                    Provider = lpt.Content_element.Provider,
                    Name = GetValuesForImportLanguages(lpt.Content_element.Name),
                    Description = GetValuesForImportLanguages(lpt.Content_element.Description),
                    Keywords = GetGroupedValuesForImportLanguages(lpt.Content_element.Keywords),
                    Category = lpt.Content_element.Content_category,
                    ReleaseDetails = GetReleaseDetails(lpt),
                    CopyrightNotices = GetValuesForImportLanguages(lpt.Content_element.Copyright_notices),
                    ProjectUuid = lpt.Project_uuid,
                    ProjectName = GetValuesForImportLanguages(lpt.Project_name),
                    CollectionUuid = lpt.Collection_uuid,
                    CollectionName = GetValuesForImportLanguages(lpt.Collection_name),
                    LicenseeUuid = lpt.Licensee_uuid,
                    LicenseeName = lpt.Licensee_name,
                    LicenseType = lpt.Offering.License_type,
                    LicenseText = GetValuesForImportLanguages(lpt.Offering.License_text.Effective_text),
                    LicenseOptions = GetLicenseOptions(lpt),
                    UsageConstraints = GetUsageConstraints(lpt),
                    DownloadConstraints = GetDownloadConstraints(lpt),
                    EffectiveIsEditorialUse = isEditorialUse,
                    SmintIoUrl = url,
                    PurchasedAt = lpt.Purchased_at,
                    CreatedAt = lpt.Created_at,
                    LptLastUpdatedAt = lpt.Last_updated_at ?? lpt.Created_at ?? DateTimeOffset.Now,
                };

                if (lpt.Can_be_synced ?? false)
                {
                    var downloadUrls =
                        await _clapicOpenApiClient.GetRawDownloadLicensePurchaseTransactionUrlsAsync(asset.CPTUuid, asset.LPTUuid);

                    asset.RawDownloadUrls = GetRawDownloadUrls(downloadUrls);

                    assets.Add(asset);
                }
            }

            return (assets, syncLptQueryResult.Continuation_uuid);
        }

        private List<SmintIoRawDownloadUrl> GetRawDownloadUrls(IList<RawDownloadUrl> rawDownloadUrls)
        {
            List<SmintIoRawDownloadUrl> smintIoRawDownloadUrls = new List<SmintIoRawDownloadUrl>();

            foreach (var rawDownloadUrl in rawDownloadUrls)
            {
                smintIoRawDownloadUrls.Add(new SmintIoRawDownloadUrl()
                {
                    FileUuid = rawDownloadUrl.File_uuid,
                    DownloadUrl = rawDownloadUrl.Download_url,
                    RecommendedFileName = rawDownloadUrl.Recommended_file_name,
                    Usage = GetValuesForImportLanguages(rawDownloadUrl.Usage)
                });
            }

            return smintIoRawDownloadUrls;
        }

        private List<SmintIoLicenseOptions> GetLicenseOptions(SyncLicensePurchaseTransaction lpt)
        {
            if (!(lpt.Offering.Has_options ?? false))
            {
                return null;
            }

            List<SmintIoLicenseOptions> options = new List<SmintIoLicenseOptions>();

            foreach (var option in lpt.Offering.Options)
            {
                options.Add(new SmintIoLicenseOptions()
                {
                    OptionName = GetValuesForImportLanguages(option.Option_name),
                    LicenseText = GetValuesForImportLanguages(option.License_text.Effective_text)
                });
            }

            return options;
        }

        private List<SmintIoUsageConstraints> GetUsageConstraints(SyncLicensePurchaseTransaction lpt)
        {
            if (lpt.License_usage_constraints == null || lpt.License_usage_constraints.Count == 0)
            {
                return null;
            }

            List<SmintIoUsageConstraints> licenseUsageConstraints = new List<SmintIoUsageConstraints>();

            foreach (var licenseUsageConstraint in lpt.License_usage_constraints)
            {
                licenseUsageConstraints.Add(new SmintIoUsageConstraints()
                {
                    EffectiveIsExclusive = licenseUsageConstraint.Effective_is_exclusive,
                    EffectiveAllowedUsages = licenseUsageConstraint.Effective_allowed_usages,
                    EffectiveRestrictedUsages = licenseUsageConstraint.Effective_restricted_usages,
                    EffectiveAllowedSizes = licenseUsageConstraint.Effective_allowed_sizes,
                    EffectiveRestrictedSizes = licenseUsageConstraint.Effective_restricted_sizes,
                    EffectiveAllowedPlacements = licenseUsageConstraint.Effective_allowed_placements,
                    EffectiveRestrictedPlacements = licenseUsageConstraint.Effective_restricted_placements,
                    EffectiveAllowedDistributions = licenseUsageConstraint.Effective_allowed_distributions,
                    EffectiveRestrictedDistributions = licenseUsageConstraint.Effective_restricted_distributions,
                    EffectiveAllowedGeographies = licenseUsageConstraint.Effective_allowed_geographies,
                    EffectiveRestrictedGeographies = licenseUsageConstraint.Effective_restricted_geographies,
                    EffectiveAllowedVerticals = licenseUsageConstraint.Effective_allowed_verticals,
                    EffectiveRestrictedVerticals = licenseUsageConstraint.Effective_restricted_verticals,
                    EffectiveToBeUsedUntil = licenseUsageConstraint.Effective_to_be_used_until,
                    EffectiveValidFrom = licenseUsageConstraint.Effective_valid_from,
                    EffectiveValidUntil = licenseUsageConstraint.Effective_valid_until,
                    EffectiveIsEditorialUse = licenseUsageConstraint.Effective_is_editorial_use
                });
            }

            return licenseUsageConstraints;
        }

        private SmintIoDownloadConstraints GetDownloadConstraints(SyncLicensePurchaseTransaction lpt)
        {
            if (lpt.License_download_constraints == null)
            {
                return null;
            }

            var licenseDownloadConstraints = lpt.License_download_constraints;

            return new SmintIoDownloadConstraints()
            {
                EffectiveMaxDownloads = licenseDownloadConstraints.Effective_max_downloads,
                EffectiveMaxUsers = licenseDownloadConstraints.Effective_max_users,
                EffectiveMaxReuses = licenseDownloadConstraints.Effective_max_reuses
            };
        }

        private SmintIoReleaseDetails GetReleaseDetails(SyncLicensePurchaseTransaction lpt)
        {
            if (lpt.Content_element.Release_details == null)
            {
                return null;
            }

            var releaseDetails = lpt.Content_element.Release_details;

            return new SmintIoReleaseDetails()
            {
                ModelReleaseState = releaseDetails.Model_release_state,
                PropertyReleaseState = releaseDetails.Property_release_state,
                ProviderAllowedUseComment = GetValuesForImportLanguages(releaseDetails.Provider_allowed_use_comment),
                ProviderReleaseComment = GetValuesForImportLanguages(releaseDetails.Provider_release_comment),
                ProviderUsageConstraints = GetValuesForImportLanguages(releaseDetails.Provider_usage_constraints)
            };
        }

        private IDictionary<string, string[]> GetGroupedValuesForImportLanguages(LocalizedMetadataElements localizedMetadataElements)
        {
            if (localizedMetadataElements == null)
                return null;

            return localizedMetadataElements
                .Where(localizedMetadataElement => _options.ImportLanguages.Contains(localizedMetadataElement.Culture))
                .GroupBy(localizedMetadataElement => localizedMetadataElement.Culture)
                .ToDictionary(group => group.Key, group => group.Select(localizedMetadataElement => localizedMetadataElement.Metadata_element.Name).ToArray());
        }

        private IDictionary<string, string> GetValuesForImportLanguages(LocalizedStrings localizedStrings)
        {
            if (localizedStrings == null)
                return null;

            return localizedStrings
                .Where(localizedString => _options.ImportLanguages.Contains(localizedString.Culture))
                .ToDictionary(localizedString => localizedString.Culture, localizedString => localizedString.Value);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _http?.Dispose();
            }

            _disposed = true;
        }
    }
}
