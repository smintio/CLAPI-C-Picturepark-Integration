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
using System.IO;
using System.Globalization;

namespace Client.Providers.Impl
{
    public sealed class SmintIoApiClientProviderImpl: IDisposable, ISmintIoApiClientProvider
    {
        private const string ENGLISH_CULTURE_CODE = "en";

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

            var englishGenericMetadata = await _retryPolicy.ExecuteAsync(async () =>
                await _clapicOpenApiClient.GetGenericMetadataAsync(ENGLISH_CULTURE_CODE));

            var cultureCount = englishGenericMetadata?.Cultures.Count ?? 0;

            if (cultureCount == 0)
                throw new Exception("No Smint.io cultures found");            

            var multiCultureGenericMetadata = new Dictionary<string, GenericMetadata>();

            multiCultureGenericMetadata.Add(ENGLISH_CULTURE_CODE, englishGenericMetadata);

            foreach (var culture in englishGenericMetadata.Cultures)
            {
                string cultureKey = culture.Key;

                if (string.Equals(cultureKey, ENGLISH_CULTURE_CODE))
                    continue;

                if (!_options.ImportLanguages.Contains(cultureKey))
                    continue;

                var cultureGenericMetadata = await _retryPolicy.ExecuteAsync(async () =>
                    await _clapicOpenApiClient.GetGenericMetadataAsync(cultureKey));

                multiCultureGenericMetadata.Add(cultureKey, cultureGenericMetadata);
            }

            var smintIoGenericMetadata = new SmintIoGenericMetadata();

            smintIoGenericMetadata.ContentProviders = GetGenericMetadataForImportLanguages(englishGenericMetadata, multiCultureGenericMetadata, (genericMetadata) => genericMetadata.Providers);
            smintIoGenericMetadata.ContentCategories = GetGenericMetadataForImportLanguages(englishGenericMetadata, multiCultureGenericMetadata, (genericMetadata) => genericMetadata.Content_categories);

            smintIoGenericMetadata.LicenseTypes = GetGenericMetadataForImportLanguages(englishGenericMetadata, multiCultureGenericMetadata, (genericMetadata) => genericMetadata.License_types);
            smintIoGenericMetadata.ReleaseStates = GetGenericMetadataForImportLanguages(englishGenericMetadata, multiCultureGenericMetadata, (genericMetadata) => genericMetadata.Release_states);

            smintIoGenericMetadata.LicenseUsages = GetGenericMetadataForImportLanguages(englishGenericMetadata, multiCultureGenericMetadata, (genericMetadata) => genericMetadata.License_usages);
            smintIoGenericMetadata.LicenseSizes = GetGenericMetadataForImportLanguages(englishGenericMetadata, multiCultureGenericMetadata, (genericMetadata) => genericMetadata.License_sizes);
            smintIoGenericMetadata.LicensePlacements = GetGenericMetadataForImportLanguages(englishGenericMetadata, multiCultureGenericMetadata, (genericMetadata) => genericMetadata.License_placements);
            smintIoGenericMetadata.LicenseDistributions = GetGenericMetadataForImportLanguages(englishGenericMetadata, multiCultureGenericMetadata, (genericMetadata) => genericMetadata.License_distributions);
            smintIoGenericMetadata.LicenseGeographies = GetGenericMetadataForImportLanguages(englishGenericMetadata, multiCultureGenericMetadata, (genericMetadata) => genericMetadata.License_geographies);
            smintIoGenericMetadata.LicenseVerticals = GetGenericMetadataForImportLanguages(englishGenericMetadata, multiCultureGenericMetadata, (genericMetadata) => genericMetadata.License_verticals);

            _logger.LogInformation("Received generic metadata from Smint.io");

            return smintIoGenericMetadata;
        }

        private IList<SmintIoMetadataElement> GetGenericMetadataForImportLanguages(GenericMetadata englishGenericMetadata, IDictionary<string, GenericMetadata> multiCultureGenericMetadata, Func<GenericMetadata, ICollection<MetadataElement>> genericMetadataElementsFunc)
        {
            var result = new List<SmintIoMetadataElement>();

            var englishGenericMetadataElements = genericMetadataElementsFunc.Invoke(englishGenericMetadata);

            if (englishGenericMetadataElements == null || !englishGenericMetadataElements.Any())
                return result;

            foreach (var englishGenericMetadataElement in englishGenericMetadataElements)
            {
                var key = englishGenericMetadataElement.Key;
                var values = new Dictionary<string, string>();

                foreach (var culture in multiCultureGenericMetadata.Keys)
                {
                    if (!_options.ImportLanguages.Contains(culture))
                        continue;

                    var cultureGenericMetadata = multiCultureGenericMetadata[culture];

                    var cultureGenericMetadataElements = genericMetadataElementsFunc.Invoke(cultureGenericMetadata);

                    var cultureGenericMetadataElement = cultureGenericMetadataElements.FirstOrDefault(
                        cultureGenericMetadataElementInner => string.Equals(cultureGenericMetadataElementInner.Key, key));

                    if (cultureGenericMetadataElement != null)
                    {
                        values.Add(culture, cultureGenericMetadataElement.Name);
                    }
                }

                var smintIoMetadataElement = new SmintIoMetadataElement()
                {
                    Key = key,
                    Values = values
                };

                result.Add(smintIoMetadataElement);
            }

            return result;
        }

        public async Task<IEnumerable<SmintIoAsset>> GetAssetsAsync(DateTimeOffset? minDate)
        {
            _logger.LogInformation("Receiving assets from Smint.io...");

            var result = await LoadAssetsAsync(minDate);
            
            _logger.LogInformation($"Received {result.Count()} assets from Smint.io");

            return result;
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
                            var result = await _authenticator.RefreshSmintIoTokenAsync(_authDataProvider.SmintIo.RefreshToken);

                            _authDataProvider.SmintIo.AccessToken = result.AccessToken;
                        }
                    });
        }

        private async Task<IEnumerable<SmintIoAsset>> LoadAssetsAsync(DateTimeOffset? minDate)
        {
            _clapicOpenApiClient.AccessToken = _authDataProvider.SmintIo.AccessToken;

            IList<SmintIoAsset> assets = new List<SmintIoAsset>();

            SyncLicensePurchaseTransactionQueryResult syncLptQueryResult = await _retryPolicy.ExecuteAsync(async () =>
            {
                return await _clapicOpenApiClient.GetLicensePurchaseTransactionsForSyncAsync(
                    lastUpdatedAtFrom: minDate,
                    limit: 10);
            });

            if (syncLptQueryResult.Count == 0)
            {
                return assets;
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
                    CartPTUuid = lpt.Cart_purchase_transaction_uuid,
                    State = lpt.State,
                    Provider = lpt.Content_element.Provider,
                    Name = GetValuesForImportLanguages(lpt.Content_element.Name),
                    Description = GetValuesForImportLanguages(lpt.Content_element.Description),
                    Keywords = GetGroupedValuesForImportLanguages(lpt.Content_element.Keywords),
                    Category = lpt.Content_element.Content_category,
                    ReleaseDetail = GetReleaseDetails(lpt),
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
                    var downloadUrl =
                        await _clapicOpenApiClient.GetRawDownloadLicensePurchaseTransactionUrlAsync(asset.CartPTUuid,
                            asset.LPTUuid);

                    asset.DownloadUrl = downloadUrl;

                    assets.Add(asset);
                }
            }

            return assets;
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

            List<SmintIoUsageConstraints> constraints = new List<SmintIoUsageConstraints>();

            foreach (var constraint in lpt.License_usage_constraints)
            {
                constraints.Add(new SmintIoUsageConstraints()
                {
                    EffectiveIsExclusive = constraint.Effective_is_exclusive,
                    EffectiveAllowedUsages = constraint.Effective_allowed_usages,
                    EffectiveRestrictedUsages = constraint.Effective_restricted_usages,
                    EffectiveAllowedSizes = constraint.Effective_allowed_sizes,
                    EffectiveRestrictedSizes = constraint.Effective_restricted_sizes,
                    EffectiveAllowedPlacements = constraint.Effective_allowed_placements,
                    EffectiveRestrictedPlacements = constraint.Effective_restricted_placements,
                    EffectiveAllowedDistributions = constraint.Effective_allowed_distributions,
                    EffectiveRestrictedDistributions = constraint.Effective_restricted_distributions,
                    EffectiveAllowedGeographies = constraint.Effective_allowed_geographies,
                    EffectiveRestrictedGeographies = constraint.Effective_restricted_geographies,
                    EffectiveAllowedVerticals = constraint.Effective_allowed_verticals,
                    EffectiveRestrictedVerticals = constraint.Effective_restricted_verticals,
                    EffectiveToBeUsedUntil = constraint.Effective_to_be_used_until,
                    EffectiveValidFrom = constraint.Effective_valid_from,
                    EffectiveValidUntil = constraint.Effective_valid_until,
                    EffectiveIsEditorialUse = constraint.Effective_is_editorial_use
                });
            }

            return constraints;
        }

        private SmintIoDownloadConstraints GetDownloadConstraints(SyncLicensePurchaseTransaction lpt)
        {
            if (lpt.License_download_constraints == null)
            {
                return null;
            }

            return new SmintIoDownloadConstraints()
            {
                EffectiveMaxDownloads = lpt.License_download_constraints.Effective_max_downloads,
                EffectiveMaxUsers = lpt.License_download_constraints.Effective_max_users,
                EffectiveMaxReuses = lpt.License_download_constraints.Effective_max_reuses
            };
        }

        private SmintIoReleaseDetail GetReleaseDetails(SyncLicensePurchaseTransaction lpt)
        {
            if (lpt.Content_element.Release_details == null)
            {
                return null;
            }

            return new SmintIoReleaseDetail()
            {
                ModelReleaseState = lpt.Content_element.Release_details.Model_release_state,
                PropertyReleaseState = lpt.Content_element.Release_details.Property_release_state,
                ProviderAllowedUseComment = GetValuesForImportLanguages(lpt.Content_element.Release_details.Provider_allowed_use_comment),
                ProviderReleaseComment = GetValuesForImportLanguages(lpt.Content_element.Release_details.Provider_release_comment),
                ProviderUsageConstraints = GetValuesForImportLanguages(lpt.Content_element.Release_details.Provider_usage_constraints)
            };
        }

        private IDictionary<string, string[]> GetGroupedValuesForImportLanguages(LocalizedMetadataElements metadataElements)
        {
            return metadataElements.Where(str => _options.ImportLanguages.Contains(str.Culture))
                .GroupBy(str => str.Culture)
                .ToDictionary(group => group.Key, group => group.Select(localStr => localStr.Metadata_element.Name).ToArray());
        }

        private IDictionary<string, string> GetValuesForImportLanguages(LocalizedStrings localizedStrings)
        {
            return localizedStrings.Where(str => _options.ImportLanguages.Contains(str.Culture))
                .ToDictionary(str => str.Culture, str => str.Value);
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
