﻿using Client.Providers;
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

            var folderName = Folder + new Random().Next(1000000, 9999999);

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

            foreach (var asset in assets)
            {
                var targetAsset = new PictureparkAsset
                {
                    TransferId = asset.CartPTUuid,
                    DownloadUrl = asset.DownloadUrl,
                    Id = asset.LPTUuid,
                    Metadata = new DataDictionary()
                    {
                        {nameof(SmintIoContentLayer), GetContentMetadata(asset)},
                        {nameof(SmintIoLicenseLayer), GetLicenseMetadata(asset)}
                    }
                };

                targetAssets.Add(targetAsset);
            }
            return targetAssets;
        } 
        */

        private async Task<IEnumerable<PictureparkAsset>> TransformAssetsAsync(IEnumerable<SmintIoAsset> assets, string folderName)
        {
            IList<PictureparkAsset> targetAssets = new List<PictureparkAsset>();

            foreach (var asset in assets)
            {
                _logger.LogInformation($"Transforming and downloading Smint.io LPT {asset.LPTUuid}...");

                string fileName = $"{folderName}/{asset.LPTUuid}.{ExtractFileExtension(asset.DownloadUrl)}";

                var transferIdentifier = Guid.NewGuid().ToString();

                var targetAsset = new PictureparkAsset()
                {
                    TransferId = transferIdentifier,
                    RecommendedFileName = asset.RecommendedFileName,
                    DownloadUrl = fileName,
                    Id = asset.LPTUuid
                };

                await DownloadFileAsync(new Uri(targetAsset.DownloadUrl), fileName);

                targetAsset.Metadata = new DataDictionary()
                {
                    { nameof(ContentLayer), await GetContentMetadataAsync(asset) },
                    { nameof(LicenseLayer), await GetLicenseMetadataAsync(asset) }
                };

                targetAssets.Add(targetAsset);

                _logger.LogInformation($"Transformed and downloaded Smint.io LPT {asset.LPTUuid}");
            }

            return targetAssets;
        }

        private async Task<DataDictionary> GetContentMetadataAsync(SmintIoAsset asset)
        {
            var keywords = JoinValues(asset.Keywords);

            var contentProvider = await GetContentProviderPictureparkKeyAsync(asset.Provider);
            var contentCategory = await GetContentCategoryPictureparkKeyAsync(asset.Category);

            var dataDictionary = new DataDictionary
            {
                { "ContentLayer.contentProvider", new { _refId = contentProvider } },
                { "ContentLayer.name", asset.Name },
                { "ContentLayer.category", new { _refId = contentCategory } },
                { "ContentLayer.smintIoUrl", asset.SmintIoUrl },
                { "ContentLayer.purchasedAt", asset.PurchasedAt },
                { "ContentLayer.createdAt", asset.CreatedAt }
            };

            if (!string.IsNullOrEmpty(asset.ProjectUuid))
                dataDictionary.Add("ContentLayer.projectUuid", asset.ProjectUuid);

            if (asset.ProjectName?.Count > 0)
                dataDictionary.Add("ContentLayer.projectName", asset.ProjectName);

            if (!string.IsNullOrEmpty(asset.CollectionUuid))
                dataDictionary.Add("ContentLayer.collectionUuid", asset.CollectionUuid);

            if (asset.CollectionName?.Count > 0)
                dataDictionary.Add("ContentLayer.collectionName", asset.CollectionName);

            if (asset.Keywords?.Count > 0)
                dataDictionary.Add("ContentLayer.keywords", keywords);

            if (asset.Description?.Count > 0)
                dataDictionary.Add("ContentLayer.description", asset.Description);

            if (asset.CopyrightNotices?.Count > 0)
                dataDictionary.Add("ContentLayer.copyrightNotices", asset.CopyrightNotices);

            return dataDictionary;
        }

        private async Task<string> GetContentProviderPictureparkKeyAsync(string smintIoKey)
        {
            var contentProviderListItems = await _pictureparkClient.GetContentProvidersAsync();

            return contentProviderListItems.First(contentProviderListItem => string.Equals(contentProviderListItem.SmintIoKey, smintIoKey)).PictureparkListItemId;
        }

        private async Task<string> GetContentCategoryPictureparkKeyAsync(string smintIoKey)
        {
            var contentCategoryListItems = await _pictureparkClient.GetContentCategoriesAsync();

            return contentCategoryListItems.First(contentCategoryListItem => string.Equals(contentCategoryListItem.SmintIoKey, smintIoKey)).PictureparkListItemId;
        }

        private async Task<DataDictionary> GetLicenseMetadataAsync(SmintIoAsset asset)
        {
            var licenseType = await GetLicenseTypePictureparkKeyAsync(asset.LicenseType);

            var dataDictionary = new DataDictionary
            {
                { "LicenseLayer.licenseeName", asset.LicenseeName },
                { "LicenseLayer.licenseeUuid", asset.LicenseeUuid },
                { "LicenseLayer.licenseType", new { _refId = licenseType }},
                { "LicenseLayer.hasBeenCancelled", asset.State == SmintIo.CLAPI.Consumer.Client.Generated.LicensePurchaseTransactionStateEnum.Cancelled }
            };

            if (asset.LicenseText?.Count > 0)
                dataDictionary.Add("LicenseLayer.licenseText", asset.LicenseText);

            if (asset.LicenseOptions?.Count > 0)
                dataDictionary.Add("LicenseLayer.licenseOptions", GetLicenseOptions(asset.LicenseOptions));

            if (asset.UsageConstraints?.Count > 0)
                dataDictionary.Add("LicenseLayer.usageConstraints", await GetUsageConstraintsAsync(asset.UsageConstraints));

            if (asset.DownloadConstraints != null)
                dataDictionary.Add("LicenseLayer.downloadConstraints", GetDownloadConstraints(asset.DownloadConstraints));

            if (asset.ReleaseDetails != null)
                dataDictionary.Add("LicenseLayer.releaseDetails", await GetReleaseDetailsMetadataAsync(asset.ReleaseDetails));

            if (asset.EffectiveIsEditorialUse != null)
                dataDictionary.Add("LicenseLayer.effectiveIsEditorialUse", asset.EffectiveIsEditorialUse);

            return dataDictionary;
        }

        private async Task<string> GetLicenseTypePictureparkKeyAsync(string smintIoKey)
        {
            if (string.IsNullOrEmpty(smintIoKey))
                return null;

            var licenseTypeListItems = await _pictureparkClient.GetLicenseTypesAsync();

            return licenseTypeListItems.First(licenseTypeListItem => string.Equals(licenseTypeListItem.SmintIoKey, smintIoKey)).PictureparkListItemId;
        }

        private DataDictionary[] GetLicenseOptions(IList<SmintIoLicenseOptions> licenseOptions)
        {
            var dataDictionary = licenseOptions.Select(licenseOption =>
            {
                var dataDictionaryInner = new DataDictionary
                {
                    { "optionName", licenseOption.OptionName }
                };

                if (licenseOption.LicenseText?.Count > 0)
                    dataDictionaryInner.Add("licenseText", licenseOption.LicenseText);

                return dataDictionaryInner;
            }).ToArray();

            return dataDictionary;
        }

        private async Task<DataDictionary[]> GetUsageConstraintsAsync(IList<SmintIoUsageConstraints> usageContraints)
        {
            var dataDictionaries = new List<DataDictionary>();

            foreach (var usageConstraint in usageContraints)
            {
                var allowedUsages = await GetLicenseUsagesPictureparkKeysAsync(usageConstraint.EffectiveAllowedUsages);
                var restrictedUsages = await GetLicenseUsagesPictureparkKeysAsync(usageConstraint.EffectiveRestrictedUsages);

                var allowedSizes = await GetLicenseSizesPictureparkKeysAsync(usageConstraint.EffectiveAllowedSizes);
                var restrictedSizes = await GetLicenseSizesPictureparkKeysAsync(usageConstraint.EffectiveRestrictedSizes);

                var allowedPlacements = await GetLicensePlacementsPictureparkKeysAsync(usageConstraint.EffectiveAllowedPlacements);
                var restrictedPlacements = await GetLicensePlacementsPictureparkKeysAsync(usageConstraint.EffectiveRestrictedPlacements);

                var allowedDistributions = await GetLicenseDistributionsPictureparkKeysAsync(usageConstraint.EffectiveAllowedDistributions);
                var restrictedDistributions = await GetLicenseDistributionsPictureparkKeysAsync(usageConstraint.EffectiveRestrictedDistributions);

                var allowedGeographies = await GetLicenseGeographiesPictureparkKeysAsync(usageConstraint.EffectiveAllowedGeographies);
                var restrictedGeographies = await GetLicenseGeographiesPictureparkKeysAsync(usageConstraint.EffectiveRestrictedGeographies);

                var allowedVerticals = await GetLicenseVerticalsPictureparkKeysAsync(usageConstraint.EffectiveAllowedVerticals);
                var restrictedVerticals = await GetLicenseVerticalsPictureparkKeysAsync(usageConstraint.EffectiveRestrictedVerticals);

                var dataDictionary = new DataDictionary()
                {
                    { "effectiveValidFrom", usageConstraint.EffectiveValidFrom }
                };

                if (usageConstraint.EffectiveIsExclusive != null)
                    dataDictionary.Add("effectiveIsExclusive", usageConstraint.EffectiveIsExclusive);

                if (allowedUsages?.Count() > 0)
                    dataDictionary.Add("effectiveAllowedUsages", allowedUsages.Select(usage => new { _refId = usage }).ToArray());

                if (restrictedUsages?.Count() > 0)
                    dataDictionary.Add("effectiveRestrictedUsages", restrictedUsages.Select(usage => new { _refId = usage }).ToArray());

                if (allowedSizes?.Count() > 0)
                    dataDictionary.Add("effectiveAllowedSizes", allowedSizes.Select(size => new { _refId = size }).ToArray());

                if (restrictedSizes?.Count() > 0)
                    dataDictionary.Add("effectiveRestrictedSizes", restrictedSizes.Select(size => new { _refId = size }).ToArray());

                if (allowedPlacements?.Count() > 0)
                    dataDictionary.Add("effectiveAllowedPlacements", allowedPlacements.Select(placement => new { _refId = placement }).ToArray());

                if (restrictedPlacements?.Count() > 0)
                    dataDictionary.Add("effectiveRestrictedPlacements", restrictedPlacements.Select(placement => new { _refId = placement }).ToArray());

                if (allowedDistributions?.Count() > 0)
                    dataDictionary.Add("effectiveAllowedDistributions", allowedDistributions.Select(distribution => new { _refId = distribution }).ToArray());

                if (restrictedDistributions?.Count() > 0)
                    dataDictionary.Add("effectiveRestrictedDistributions", restrictedDistributions.Select(distribution => new { _refId = distribution }).ToArray());

                if (allowedGeographies?.Count() > 0)
                    dataDictionary.Add("effectiveAllowedGeographies", allowedGeographies.Select(geography => new { _refId = geography }).ToArray());

                if (restrictedGeographies?.Count() > 0)
                    dataDictionary.Add("effectiveRestrictedGeographies", restrictedGeographies.Select(geography => new { _refId = geography }).ToArray());

                if (allowedVerticals?.Count() > 0)
                    dataDictionary.Add("effectiveAllowedVerticals", allowedVerticals.Select(vertical => new { _refId = vertical }).ToArray());

                if (restrictedVerticals?.Count() > 0)
                    dataDictionary.Add("effectiveRestrictedVerticals", restrictedVerticals.Select(vertical => new { _refId = vertical }).ToArray());

                if (usageConstraint.EffectiveValidUntil != null)
                    dataDictionary.Add("effectiveValidUntil", usageConstraint.EffectiveValidUntil);

                if (usageConstraint.EffectiveToBeUsedUntil != null)
                    dataDictionary.Add("effectiveToBeUsedUntil", usageConstraint.EffectiveToBeUsedUntil);

                if (usageConstraint.EffectiveIsEditorialUse != null)
                    dataDictionary.Add("effectiveIsEditorialUse", usageConstraint.EffectiveIsEditorialUse);

                dataDictionaries.Add(dataDictionary);
            }

            return dataDictionaries.ToArray();
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

        private DataDictionary GetDownloadConstraints(SmintIoDownloadConstraints downloadConstraints)
        {
            if (downloadConstraints == null)
            {
                return null;
            }

            var dataDictionary = new DataDictionary();

            if (downloadConstraints.EffectiveMaxDownloads != null)
                dataDictionary.Add("effectiveMaxDownloads", downloadConstraints.EffectiveMaxDownloads);

            if (downloadConstraints.EffectiveMaxUsers != null)
                dataDictionary.Add("effectiveMaxUsers", downloadConstraints.EffectiveMaxUsers);

            if (downloadConstraints.EffectiveMaxReuses != null)
                dataDictionary.Add("effectiveMaxReuses", downloadConstraints.EffectiveMaxReuses);

            return dataDictionary;
        }

        private async Task<DataDictionary> GetReleaseDetailsMetadataAsync(SmintIoReleaseDetails releaseDetails)
        {
            var modelReleaseState = await GetReleaseStatePictureparkKeyAsync(releaseDetails.ModelReleaseState);
            var propertyReleaseState = await GetReleaseStatePictureparkKeyAsync(releaseDetails.PropertyReleaseState);

            var dataDictionary = new DataDictionary();

            if (!string.IsNullOrEmpty(modelReleaseState))
                dataDictionary.Add("modelReleaseState", new { _refId = modelReleaseState });

            if (!string.IsNullOrEmpty(propertyReleaseState))
                dataDictionary.Add("propertyReleaseState", new { _refId = propertyReleaseState });

            if (releaseDetails.ProviderAllowedUseComment?.Count > 0)
                dataDictionary.Add("providerAllowedUseComment", releaseDetails.ProviderAllowedUseComment);

            if (releaseDetails.ProviderReleaseComment?.Count > 0)
                dataDictionary.Add("providerReleaseComment", releaseDetails.ProviderReleaseComment);

            if (releaseDetails.ProviderUsageConstraints?.Count > 0)
                dataDictionary.Add("providerUsageConstraints", releaseDetails.ProviderUsageConstraints);

            return dataDictionary;
        }

        private async Task<string> GetReleaseStatePictureparkKeyAsync(string smintIoKey)
        {
            if (string.IsNullOrEmpty(smintIoKey))
                return null;

            var releaseStateListItems = await _pictureparkClient.GetReleaseStatesAsync();

            return releaseStateListItems.First(releaseStateListItem => string.Equals(releaseStateListItem.SmintIoKey, smintIoKey)).PictureparkListItemId;
        }

        private IDictionary<string, string> JoinValues(IDictionary<string, string[]> dictionary)
        {
            if (dictionary == null || !dictionary.Any())
            {
                return null;
            }

            var result = new Dictionary<string, string>();

            foreach (var (key, value) in dictionary)
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
