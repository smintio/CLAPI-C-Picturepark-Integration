using Client.Providers;
using Microsoft.Extensions.Logging;
using Picturepark.SDK.V1.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Client.Contracts;
using System.IO;
using System.Net;
using Client.Contracts.Picturepark;
using Client.Providers.Impl.Models;

namespace Client.Jobs.Impl
{
    public class SyncJobImpl: ISyncJob
    {
        private const string Folder = "temp";

        private static readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1, 1);

        private readonly ISyncDatabaseProvider _syncDatabaseProvider;

        private readonly ISmintIoApiClientProvider _smintIoClient;
        private readonly IPictureparkApiClientProvider _pictureparkClient;

        private readonly ILogger _logger;

        public SyncJobImpl(
            ISyncDatabaseProvider syncDatabaseProvider,
            ISmintIoApiClientProvider smintIoClient,
            IPictureparkApiClientProvider pictureparkClient,
            ILogger<SyncJobImpl> logger)
        {
            _syncDatabaseProvider = syncDatabaseProvider;

            _smintIoClient = smintIoClient;
            _pictureparkClient = pictureparkClient;

            _logger = logger;
        }

        public async Task SynchronizeAsync(bool synchronizeGenericMetadata)
        {
            await Semaphore.WaitAsync();

            try
            {
                if (synchronizeGenericMetadata)
                {
                    await SynchronizeGenericMetadataAsync();
                }

                await SynchronizeAssetsAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in sync job");
            }
            finally
            {
                Semaphore.Release();
            }
        }

        private async Task SynchronizeGenericMetadataAsync()
        {
            _logger.LogInformation("Starting Smint.io generic metadata synchronization...");

            var genericMetadata = await _smintIoClient.GetGenericMetadataAsync();

            var transformedContentProviders = TransformGenericMetadata(genericMetadata.ContentProviders);
            await _pictureparkClient.ImportContentProvidersAsync(transformedContentProviders);

            var transformedContentCategories = TransformGenericMetadata(genericMetadata.ContentCategories);
            await _pictureparkClient.ImportContentCategoriesAsync(transformedContentCategories);

            var transformedLicenseTypes = TransformGenericMetadata(genericMetadata.LicenseTypes);
            await _pictureparkClient.ImportLicenseTypesAsync(transformedLicenseTypes);

            var transformedReleaseStates = TransformGenericMetadata(genericMetadata.ReleaseStates);
            await _pictureparkClient.ImportReleaseStatesAsync(transformedReleaseStates);

            var transformedLicenseUsages = TransformGenericMetadata(genericMetadata.LicenseUsages);
            await _pictureparkClient.ImportLicenseUsagesAsync(transformedLicenseUsages);

            var transformedLicenseSizes = TransformGenericMetadata(genericMetadata.LicenseSizes);
            await _pictureparkClient.ImportLicenseSizesAsync(transformedLicenseSizes);

            var transformedLicensePlacements = TransformGenericMetadata(genericMetadata.LicensePlacements);
            await _pictureparkClient.ImportLicensePlacementsAsync(transformedLicensePlacements);

            var transformedLicenseDistributions = TransformGenericMetadata(genericMetadata.LicenseDistributions);
            await _pictureparkClient.ImportLicenseDistributionsAsync(transformedLicenseDistributions);

            var transformedLicenseGeographies = TransformGenericMetadata(genericMetadata.LicenseGeographies);
            await _pictureparkClient.ImportLicenseGeographiesAsync(transformedLicenseGeographies);

            var transformedLicenseVerticals = TransformGenericMetadata(genericMetadata.LicenseVerticals);
            await _pictureparkClient.ImportLicenseVerticalsAsync(transformedLicenseVerticals);

            _pictureparkClient.ClearCache();

            _logger.LogInformation("Finished Smint.io generic metadata synchronization");
        }

        private IEnumerable<PictureparkListItem> TransformGenericMetadata(IList<SmintIoMetadataElement> smintIoGenericMetadataElements)
        {
            return smintIoGenericMetadataElements.Select(smintIoGenericMetadataElement =>
            {
                return new PictureparkListItem()
                {
                    SmintIoKey = smintIoGenericMetadataElement.Key,
                    Content = new DataDictionary()
                    {
                        { "key", smintIoGenericMetadataElement.Key },
                        { "name", smintIoGenericMetadataElement.Values }
                    }
                };
            });
        }

        private async Task SynchronizeAssetsAsync()
        {
            _logger.LogInformation("Starting Smint.io asset synchronization...");

            var folderName = Folder + new Random().Next(1000, 9999);

            var syncDatabaseModel = _syncDatabaseProvider.GetSyncDatabaseModel();

            DateTimeOffset? minDate = null;

            if (syncDatabaseModel != null)
            {
                // get last committed state

                minDate = syncDatabaseModel.NextMinDate;
            }

            var originalMinDate = minDate;

            try
            {
                IEnumerable<SmintIoAsset> assets = null;

                do
                {
                    assets = await _smintIoClient.GetAssetsAsync(minDate);

                    if (assets != null && assets.Any())
                    {
                        CreateTempFolder(folderName);

                        minDate = assets.Max(asset => asset.LptLastUpdatedAt);

                        var transformedAssets = await TransformAssetsAsync(assets, folderName);

                        await _pictureparkClient.ImportAssetsAsync(transformedAssets);
                    }

                    // store committed data

                    _syncDatabaseProvider.SetSyncDatabaseModel(new SyncDatabaseModel()
                    {
                        NextMinDate = minDate
                    });

                    _logger.LogInformation($"Synchronized {assets.Count()} Smint.io assets");
                } while (assets != null && assets.Any());

                _logger.LogInformation("Finished Smint.io asset synchronization");
            }
            finally
            {
                RemoveTempFolder(folderName);
            }
        }

        private void CreateTempFolder(string folderName)
        {
            Directory.CreateDirectory(folderName);
        }

        private void RemoveTempFolder(string folderName)
        {
            if (Directory.Exists(folderName))
                Directory.Delete(folderName, true);
        }

        /*
         * Directly transfer the asset binary data from Smint.io to Picturepark
         * This is not working currently, some error occurs on Picturepark side
         */

        /* private IEnumerable<PictureparkAsset> TransformAssetsWeb(IEnumerable<SmintIoAsset> assets, string folderName)
        {
            IList<PictureparkAsset> targetAssets = new List<PictureparkAsset>();
            foreach (var smintAsset in assets)
            {
                var ppAsset = new PictureparkAsset
                {
                    TransferId = smintAsset.CartPTUuid,
                    DownloadUrl = smintAsset.DownloadUrl,
                    Id = smintAsset.LPTUuid,
                    Metadata = new DataDictionary()
                    {
                        {nameof(SmintIoContentLayer), GetContentMetaData(smintAsset)},
                        {nameof(SmintIoLicenseLayer), GetLicenseMetaData(smintAsset)}
                    }
                };


                targetAssets.Add(ppAsset);
            }
            return targetAssets;
        } */

        private async Task<IEnumerable<PictureparkAsset>> TransformAssetsAsync(IEnumerable<SmintIoAsset> assets, string folderName)
        {
            IList<PictureparkAsset> targetAssets = new List<PictureparkAsset>();

            foreach (var smintAsset in assets)
            {
                _logger.LogInformation($"Transforming and downloading Smint.io LPT {smintAsset.LPTUuid}...");

                string fileName = $"{folderName}/{smintAsset.LPTUuid}.{ExtractFileExtension(smintAsset.DownloadUrl)}";

                var transferIdentifier = $"SMINTIO_LPT_{smintAsset.LPTUuid}";

                var ppAsset = new PictureparkAsset()
                {
                    TransferId = transferIdentifier,
                    DownloadUrl = fileName,
                    Id = smintAsset.LPTUuid
                };

                await DownloadFileAsync(new Uri(smintAsset.DownloadUrl), fileName);

                ppAsset.Metadata = new DataDictionary()
                {
                    { nameof(ContentLayer), await GetContentMetaDataAsync(smintAsset) },
                    { nameof(LicenseLayer), await GetLicenseMetaDataAsync(smintAsset) }
                };

                targetAssets.Add(ppAsset);

                _logger.LogInformation($"Transformed and downloaded Smint.io LPT {smintAsset.LPTUuid}");
            }

            return targetAssets;
        }

        private async Task<DataDictionary> GetContentMetaDataAsync(SmintIoAsset asset)
        {
            var keywords = JoinValues(asset.Keywords);

            var contentProvider = await GetContentProviderPictureparkKeyAsync(asset.Provider);
            var contentCategory = await GetContentCategoryPictureparkKeyAsync(asset.Category);

            return new DataDictionary
            {
                { "provider", contentProvider },
                { "name", asset.Name?.Count > 0 ? asset.Name : null },
                { "description", asset.Description?.Count > 0 ? asset.Description : null },
                { "keywords", keywords?.Count > 0 ? keywords : null },
                { "category", contentCategory },
                { "copyrightNotices", asset.CopyrightNotices?.Count > 0 ? asset.CopyrightNotices : null },
                { "projectUuid", asset.ProjectUuid },
                { "projectName", asset.ProjectName },
                { "collectionUuid", asset.CollectionUuid },
                { "collectionName", asset.CollectionName },
                { "smintIoUrl", asset.SmintIoUrl },
                { "purchasedAt", asset.PurchasedAt },
                { "createdAt", asset.CreatedAt }
            };
        }

        private async Task<string> GetContentProviderPictureparkKeyAsync(string smintIoKey)
        {
            if (string.IsNullOrEmpty(smintIoKey))
                return null;

            var contentProviderListItems = await _pictureparkClient.GetContentProvidersAsync();

            return contentProviderListItems.First(contentProviderListItem => string.Equals(contentProviderListItem.SmintIoKey, smintIoKey)).PictureparkListItemId;
        }

        private async Task<string> GetContentCategoryPictureparkKeyAsync(string smintIoKey)
        {
            if (string.IsNullOrEmpty(smintIoKey))
                return null;

            var contentCategoryListItems = await _pictureparkClient.GetContentCategoriesAsync();

            return contentCategoryListItems.First(contentCategoryListItem => string.Equals(contentCategoryListItem.SmintIoKey, smintIoKey)).PictureparkListItemId;
        }

        private async Task<DataDictionary> GetLicenseMetaDataAsync(SmintIoAsset asset)
        {
            var licenseType = await GetLicenseTypePictureparkKeyAsync(asset.LicenseType);

            return new DataDictionary
            {
                { "licenseeName", asset.LicenseeName },
                { "licenseeUuid", asset.LicenseeUuid },
                { "licenseType", licenseType },
                { "licenseText", asset.LicenseText },
                { "licenseOptions", GetLicenseOptions(asset.LicenseOptions) },
                { "usageConstraints", await GetUsageConstraintsAsync(asset.UsageConstraints) },
                { "downloadConstraints", GetDownloadConstraints(asset.DownloadConstraints) },
                { "releaseDetails", asset.ReleaseDetail == null ? null : await GetReleaseDetailsMetaDataAsync(asset.ReleaseDetail) },
                { "effectiveIsEditorialUse", asset.EffectiveIsEditorialUse },
                { "hasBeenCancelled", asset.State == SmintIo.CLAPI.Consumer.Client.Generated.LicensePurchaseTransactionStateEnum.Cancelled }
            };
        }

        private async Task<string> GetLicenseTypePictureparkKeyAsync(string smintIoKey)
        {
            if (string.IsNullOrEmpty(smintIoKey))
                return null;

            var licenseTypeListItems = await _pictureparkClient.GetLicenseTypesAsync();

            return licenseTypeListItems.First(licenseTypeListItem => string.Equals(licenseTypeListItem.SmintIoKey, smintIoKey)).PictureparkListItemId;
        }

        private DataDictionary[] GetLicenseOptions(IList<SmintIoLicenseOptions> options)
        {
            if (options == null || !options.Any())
            {
                return null;
            }

            return options.Select(option => new DataDictionary
            {
                { "optionName", option.OptionName },
                { "licenseText", option.LicenseText },
            }).ToArray();
        }

        private async Task<DataDictionary[]> GetUsageConstraintsAsync(IList<SmintIoUsageConstraints> constraints)
        {
            if (constraints == null || !constraints.Any())
            {
                return null;
            }

            var result = new List<DataDictionary>();

            foreach (var restr in constraints)
            {
                var allowedUsages = await GetLicenseUsagesPictureparkKeysAsync(restr.EffectiveAllowedUsages);
                var restrictedUsages = await GetLicenseUsagesPictureparkKeysAsync(restr.EffectiveRestrictedUsages);

                var allowedSizes = await GetLicenseSizesPictureparkKeysAsync(restr.EffectiveAllowedSizes);
                var restrictedSizes = await GetLicenseSizesPictureparkKeysAsync(restr.EffectiveRestrictedSizes);

                var allowedPlacements = await GetLicensePlacementsPictureparkKeysAsync(restr.EffectiveAllowedPlacements);
                var restrictedPlacements = await GetLicensePlacementsPictureparkKeysAsync(restr.EffectiveRestrictedPlacements);

                var allowedDistributions = await GetLicenseDistributionsPictureparkKeysAsync(restr.EffectiveAllowedDistributions);
                var restrictedDistributions = await GetLicenseDistributionsPictureparkKeysAsync(restr.EffectiveRestrictedDistributions);

                var allowedGeographies = await GetLicenseGeographiesPictureparkKeysAsync(restr.EffectiveAllowedGeographies);
                var restrictedGeographies = await GetLicenseGeographiesPictureparkKeysAsync(restr.EffectiveRestrictedGeographies);

                var allowedVerticals = await GetLicenseVerticalsPictureparkKeysAsync(restr.EffectiveAllowedVerticals);
                var restrictedVerticals = await GetLicenseVerticalsPictureparkKeysAsync(restr.EffectiveRestrictedVerticals);

                var dataDictionary = new DataDictionary
                {
                    { "effectiveIsExclusive", restr.EffectiveIsExclusive },
                    { "effectiveAllowedUsages", allowedUsages },
                    { "effectiveRestrictedUsages", restrictedUsages },
                    { "effectiveAllowedSizes", allowedSizes },
                    { "effectiveRestrictedSizes", restrictedSizes },
                    { "effectiveAllowedPlacements", allowedPlacements },
                    { "effectiveRestrictedPlacements", restrictedPlacements },
                    { "effectiveAllowedDistributions", allowedDistributions },
                    { "effectiveRestrictedDistributions", restrictedDistributions },
                    { "effectiveAllowedGeographies", allowedGeographies },
                    { "effectiveRestrictedGeographies", restrictedGeographies },
                    { "effectiveAllowedVerticals", allowedVerticals },
                    { "effectiveRestrictedVerticals", restrictedVerticals },
                    { "effectiveValidFrom", restr.EffectiveValidFrom },
                    { "effectiveValidUntil", restr.EffectiveValidUntil },
                    { "effectiveToBeUsedUntil", restr.EffectiveToBeUsedUntil },
                    { "effectiveIsEditorialUse", restr.EffectiveIsEditorialUse }
                };

                result.Add(dataDictionary);
            }

            return result.ToArray();
        }

        private async Task<IEnumerable<string>> GetLicenseUsagesPictureparkKeysAsync(IList<string> smintIoKeys)
        {
            if (smintIoKeys == null || !smintIoKeys.Any())
                return null;

            var licenseUsagesListItems = await _pictureparkClient.GetLicenseUsagesAsync();

            return licenseUsagesListItems
                .Where(licenseUsageListItem => smintIoKeys.Any(smintIoKey => string.Equals(licenseUsageListItem.SmintIoKey, smintIoKey)))
                .Select(licenseUsageListItem => licenseUsageListItem.PictureparkListItemId);
        }

        private async Task<IEnumerable<string>> GetLicenseSizesPictureparkKeysAsync(IList<string> smintIoKeys)
        {
            if (smintIoKeys == null || !smintIoKeys.Any())
                return null;

            var licenseSizesListItems = await _pictureparkClient.GetLicenseSizesAsync();

            return licenseSizesListItems
                .Where(licenseSizeListItem => smintIoKeys.Any(smintIoKey => string.Equals(licenseSizeListItem.SmintIoKey, smintIoKey)))
                .Select(licenseSizeListItem => licenseSizeListItem.PictureparkListItemId);
        }

        private async Task<IEnumerable<string>> GetLicensePlacementsPictureparkKeysAsync(IList<string> smintIoKeys)
        {
            if (smintIoKeys == null || !smintIoKeys.Any())
                return null;

            var licensePlacementsListItems = await _pictureparkClient.GetLicensePlacementsAsync();

            return licensePlacementsListItems
                .Where(licensePlacementListItem => smintIoKeys.Any(smintIoKey => string.Equals(licensePlacementListItem.SmintIoKey, smintIoKey)))
                .Select(licensePlacementListItem => licensePlacementListItem.PictureparkListItemId);
        }

        private async Task<IEnumerable<string>> GetLicenseDistributionsPictureparkKeysAsync(IList<string> smintIoKeys)
        {
            if (smintIoKeys == null || !smintIoKeys.Any())
                return null;

            var licenseDistributionsListItems = await _pictureparkClient.GetLicenseDistributionsAsync();

            return licenseDistributionsListItems
                .Where(licenseDistributionListItem => smintIoKeys.Any(smintIoKey => string.Equals(licenseDistributionListItem.SmintIoKey, smintIoKey)))
                .Select(licenseDistributionListItem => licenseDistributionListItem.PictureparkListItemId);
        }

        private async Task<IEnumerable<string>> GetLicenseGeographiesPictureparkKeysAsync(IList<string> smintIoKeys)
        {
            if (smintIoKeys == null || !smintIoKeys.Any())
                return null;

            var licenseGeographiesListItems = await _pictureparkClient.GetLicenseGeographiesAsync();

            return licenseGeographiesListItems
                .Where(licenseGeographyListItem => smintIoKeys.Any(smintIoKey => string.Equals(licenseGeographyListItem.SmintIoKey, smintIoKey)))
                .Select(licenseGeographyListItem => licenseGeographyListItem.PictureparkListItemId);
        }

        private async Task<IEnumerable<string>> GetLicenseVerticalsPictureparkKeysAsync(IList<string> smintIoKeys)
        {
            if (smintIoKeys == null || !smintIoKeys.Any())
                return null;

            var licenseVerticalsListItems = await _pictureparkClient.GetLicenseVerticalsAsync();

            return licenseVerticalsListItems
                .Where(licenseVerticalListItem => smintIoKeys.Any(smintIoKey => string.Equals(licenseVerticalListItem.SmintIoKey, smintIoKey)))
                .Select(licenseVerticalListItem => licenseVerticalListItem.PictureparkListItemId);
        }

        private DataDictionary GetDownloadConstraints(SmintIoDownloadConstraints constraint)
        {
            if (constraint == null)
            {
                return null;
            }

            return new DataDictionary()
            {
                {"effectiveMaxDownloads", constraint.EffectiveMaxDownloads},
                {"effectiveMaxUsers", constraint.EffectiveMaxUsers},
                {"effectiveMaxReuses", constraint.EffectiveMaxReuses},                
            };
        }

        private async Task<DataDictionary> GetReleaseDetailsMetaDataAsync(SmintIoReleaseDetail detail)
        {
            var modelReleaseState = await GetReleaseStatePictureparkKeyAsync(detail.ModelReleaseState);
            var propertyReleaseState = await GetReleaseStatePictureparkKeyAsync(detail.PropertyReleaseState);

            return new DataDictionary()
            {
                { "modelReleaseState", modelReleaseState },
                { "propertyReleaseState", propertyReleaseState },
                { "providerAllowedUseComment", detail.ProviderAllowedUseComment?.Count > 0 ? detail.ProviderAllowedUseComment : null },
                { "providerReleaseComment", detail.ProviderReleaseComment?.Count > 0 ? detail.ProviderReleaseComment : null },
                { "providerUsageConstraints", detail.ProviderUsageConstraints?.Count > 0 ? detail.ProviderUsageConstraints : null }
            };
        }

        private async Task<string> GetReleaseStatePictureparkKeyAsync(string smintIoKey)
        {
            if (string.IsNullOrEmpty(smintIoKey))
                return null;

            var releaseStateListItems = await _pictureparkClient.GetReleaseStatesAsync();

            return releaseStateListItems.First(releaseStateListItem => string.Equals(releaseStateListItem.SmintIoKey, smintIoKey)).PictureparkListItemId;
        }

        private IDictionary<string, string> JoinValues(IDictionary<string, string[]> dict)
        {
            if (dict == null || !dict.Any())
            {
                return null;
            }

            var result = new Dictionary<string, string>();

            foreach (var (key, value) in dict)
            {
                string joinedValues = String.Join(", ", value);

                result[key] = joinedValues;
            }

            return result;
        }

        private string ExtractFileExtension(string url)
        {
            url = url.Substring(0, url.IndexOf("?"));

            string fileName = url.Substring(url.LastIndexOf("/") + 1);

            return fileName.Substring(fileName.LastIndexOf(".") + 1);
        }

        private async Task DownloadFileAsync(Uri uri, string fileName)
        {
            try
            {
                WebClient wc = new WebClient();

                await wc.DownloadFileTaskAsync(uri, fileName);
            }
            catch (WebException we)
            {
                _logger.LogError(we, "Error downloading asset");

                throw;
            }
        }
    }
}
