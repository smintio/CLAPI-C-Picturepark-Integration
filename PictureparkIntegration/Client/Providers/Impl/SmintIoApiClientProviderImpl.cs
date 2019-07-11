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
        }

        public async Task<IEnumerable<SmintIoAsset>> GetAssetsAsync(DateTimeOffset? minDate)
        {
            _logger.LogInformation($"Receiving assets from SmintIo...");

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
            var cptClient = new CLAPICOpenApiClient(_http);

            cptClient.BaseUrl = $"https://{_options.TenantId}-clapi.smint.io/consumer/v1";

            cptClient.AccessToken = _authDataProvider.SmintIo.AccessToken;

            IList<SmintIoAsset> assets = new List<SmintIoAsset>();

            SyncLicensePurchaseTransactionQueryResult syncLptQueryResult = await _retryPolicy.ExecuteAsync(async () =>
            {
                return await cptClient.GetLicensePurchaseTransactionsForSyncAsync(
                    lastUpdatedAtFrom: minDate,
                    limit: 10);
            });

            if (syncLptQueryResult.Count == 0)
            {
                return assets;
            }

            foreach (var lpt in syncLptQueryResult.License_purchase_transactions)
            {
                var licenseUsageRestrictions = lpt.License_usage_restrictions
                    .Select(lur => lur.Effective_is_editorial_use ?? false)
                    .ToList();

                bool isEditorialUse = licenseUsageRestrictions.Contains(true);

                string url = $"https://{_options.TenantId}.smint.io/project/{lpt.Project_uuid}/content-element/{lpt.Content_element.Uuid}";

                var asset = new SmintIoAsset()
                {
                    LPTUuid = lpt.Uuid,
                    CartPTUuid = lpt.Cart_purchase_transaction_uuid,
                    Provider = lpt.Content_element.Provider,
                    Name = GetValuesForImportLanguages(lpt.Content_element.Name),
                    Description = GetValuesForImportLanguages(lpt.Content_element.Description),
                    Keywords = GetGroupedValuesForImportLanguages(lpt.Content_element.Keywords),
                    Categories = GetGroupedValuesForImportLanguages(lpt.Content_element.Categories),
                    ReleaseDetail = GetReleaseDetails(lpt),
                    CopyrightNotices = GetValuesForImportLanguages(lpt.Content_element.Copyright_notices),
                    ProjectUuid = lpt.Project_uuid,
                    ProjectName = GetValuesForImportLanguages(lpt.Project_name),
                    CollectionUuid = lpt.Collection_uuid,
                    CollectionName = GetValuesForImportLanguages(lpt.Collection_name),
                    LicenseeUuid = lpt.Licensee_uuid,
                    LicenseeName = lpt.Licensee_name,
                    LicenseType = lpt.Offering.License_type,
                    LicenseOptions = GetLicenseOptions(lpt),
                    UsageRestrictions = GetUsageRestrictions(lpt),
                    DownloadRestrictions = GetDownloadRestrictions(lpt),
                    EffectiveIsEditorialUse = isEditorialUse,
                    SmintIoUrl = url,                    
                    PurchasedAt = lpt.Purchased_at,
                    CreatedAt = lpt.Created_at,
                    LptLastUpdatedAt = lpt.Last_updated_at ?? lpt.Created_at ?? DateTimeOffset.Now,
                };

                if (lpt.Can_be_synced ?? false)
                {
                    var downloadUrl =
                        await cptClient.GetRawDownloadLicensePurchaseTransactionUrlAsync(asset.CartPTUuid,
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

        private List<SmintIoUsageRestrictions> GetUsageRestrictions(SyncLicensePurchaseTransaction lpt)
        {
            if (lpt.License_usage_restrictions == null || lpt.License_usage_restrictions.Count == 0)
            {
                return null;
            }

            List<SmintIoUsageRestrictions> restrictions = new List<SmintIoUsageRestrictions>();

            foreach (var restriction in lpt.License_usage_restrictions)
            {
                restrictions.Add(new SmintIoUsageRestrictions()
                {
                    EffectiveAllowedGeographies = restriction.Effective_allowed_geographies != null ? String.Join(",", restriction.Effective_allowed_geographies) : null,
                    EffectiveRestrictedGeographies = restriction.Effective_allowed_geographies != null ? String.Join(",", restriction.Effective_restricted_geographies) : null,
                    EffectiveToBeUsedUntil = restriction.Effective_to_be_used_until,
                    EffectiveValidFrom = restriction.Effective_valid_from,
                    EffectiveValidUntil =  restriction.Effective_valid_until                  
                });
            }

            return restrictions;
        }

        private SmintIoDownloadRestrictions GetDownloadRestrictions(SyncLicensePurchaseTransaction lpt)
        {
            if (lpt.License_download_restrictions == null)
            {
                return null;
            }

            return new SmintIoDownloadRestrictions()
            {
                EffectiveMaxUsers = lpt.License_download_restrictions.Effective_max_usages,
                EffectiveMaxUsages = lpt.License_download_restrictions.Effective_max_usages,
                EffectiveMaxDownloads = lpt.License_download_restrictions.Effective_max_downloads
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
                ProviderUsageRestrictions = GetValuesForImportLanguages(lpt.Content_element.Release_details.Provider_usage_restrictions)
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
