using Client.Contracts;
using Client.Contracts.Picturepark;
using Client.Providers;
using Microsoft.Extensions.Logging;
using Picturepark.SDK.V1.Contract;
using SmintIo.CLAPI.Consumer.Integration.Core.Contracts;
using SmintIo.CLAPI.Consumer.Integration.Core.Exceptions;
using SmintIo.CLAPI.Consumer.Integration.Core.Target;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Target.Impl
{
    public class PictureparkSyncTargetImpl : ISyncTarget
    {
        private readonly IPictureparkApiClientProvider _pictureparkClient;

        private readonly ILogger<PictureparkSyncTargetImpl> _logger;

        public PictureparkSyncTargetImpl(
            IPictureparkApiClientProvider pictureparkClient,
            ILogger<PictureparkSyncTargetImpl> logger)
        {
            _pictureparkClient = pictureparkClient;

            _logger = logger;
        }

        public async Task ImportContentProvidersAsync(IList<SmintIoMetadataElement> contentProviders)
        {
            var transformedContentProviders = TransformGenericMetadata(contentProviders);
            await _pictureparkClient.ImportContentProvidersAsync(transformedContentProviders);
        }

        public async Task ImportContentTypesAsync(IList<SmintIoMetadataElement> contentTypes)
        {
            var transformedContentTypes = TransformGenericMetadata(contentTypes);
            await _pictureparkClient.ImportContentTypesAsync(transformedContentTypes);
        }

        public async Task ImportBinaryTypesAsync(IList<SmintIoMetadataElement> binaryTypes)
        {
            var transformedBinaryTypes = TransformGenericMetadata(binaryTypes);
            await _pictureparkClient.ImportBinaryTypesAsync(transformedBinaryTypes);
        }

        public async Task ImportContentCategoriesAsync(IList<SmintIoMetadataElement> contentCategories)
        {
            var transformedContentCategories = TransformGenericMetadata(contentCategories);
            await _pictureparkClient.ImportContentCategoriesAsync(transformedContentCategories);
        }

        public async Task ImportLicenseTypesAsync(IList<SmintIoMetadataElement> licenseTypes)
        {
            var transformedLicenseTypes = TransformGenericMetadata(licenseTypes);
            await _pictureparkClient.ImportLicenseTypesAsync(transformedLicenseTypes);
        }

        public async Task ImportReleaseStatesAsync(IList<SmintIoMetadataElement> releaseStates)
        {
            var transformedReleaseStates = TransformGenericMetadata(releaseStates);
            await _pictureparkClient.ImportReleaseStatesAsync(transformedReleaseStates);
        }

        public async Task ImportLicenseExclusivitiesAsync(IList<SmintIoMetadataElement> licenseExclusivities)
        {
            var transformedLicenseExclusivities = TransformGenericMetadata(licenseExclusivities);
            await _pictureparkClient.ImportLicenseExclusivitiesAsync(transformedLicenseExclusivities);
        }

        public async Task ImportLicenseUsagesAsync(IList<SmintIoMetadataElement> licenseUsages)
        {
            var transformedLicenseUsages = TransformGenericMetadata(licenseUsages);
            await _pictureparkClient.ImportLicenseUsagesAsync(transformedLicenseUsages);
        }

        public async Task ImportLicenseSizesAsync(IList<SmintIoMetadataElement> licenseSizes)
        {
            var transformedLicenseSizes = TransformGenericMetadata(licenseSizes);
            await _pictureparkClient.ImportLicenseSizesAsync(transformedLicenseSizes);
        }

        public async Task ImportLicensePlacementsAsync(IList<SmintIoMetadataElement> licensePlacements)
        {
            var transformedLicensePlacements = TransformGenericMetadata(licensePlacements);
            await _pictureparkClient.ImportLicensePlacementsAsync(transformedLicensePlacements);
        }

        public async Task ImportLicenseDistributionsAsync(IList<SmintIoMetadataElement> licenseDistributions)
        {
            var transformedLicenseDistributions = TransformGenericMetadata(licenseDistributions);
            await _pictureparkClient.ImportLicenseDistributionsAsync(transformedLicenseDistributions);
        }

        public async Task ImportLicenseGeographiesAsync(IList<SmintIoMetadataElement> licenseGeographies)
        {
            var transformedLicenseGeographies = TransformGenericMetadata(licenseGeographies);
            await _pictureparkClient.ImportLicenseGeographiesAsync(transformedLicenseGeographies);
        }

        public async Task ImportLicenseVerticalsAsync(IList<SmintIoMetadataElement> licenseVerticals)
        {
            var transformedLicenseVerticals = TransformGenericMetadata(licenseVerticals);
            await _pictureparkClient.ImportLicenseVerticalsAsync(transformedLicenseVerticals);
        }

        public async Task ImportLicenseLanguagesAsync(IList<SmintIoMetadataElement> licenseLanguages)
        {
            var transformedLicenseLanguages = TransformGenericMetadata(licenseLanguages);
            await _pictureparkClient.ImportLicenseLanguagesAsync(transformedLicenseLanguages);
        }

        private IList<PictureparkListItem> TransformGenericMetadata(IList<SmintIoMetadataElement> smintIoGenericMetadataElements)
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
            }).ToList();
        }

        public async Task ImportAssetsAsync(string folderName, IList<SmintIoAsset> assets)
        {
            var transferIdentifier = $"Smint.io Import {Guid.NewGuid().ToString()}";

            var transformedAssets = await TransformAssetsAsync(assets, transferIdentifier);

            await _pictureparkClient.ImportAssetsAsync(folderName, transformedAssets);
        }

        /*
        * Directly transfer the asset binary data from Smint.io to Picturepark
        * This is not working currently, some error occurs on Picturepark side
        */

        /* private IList<PictureparkAsset> TransformAssetsWeb(IList<SmintIoAsset> assets, string folderName)
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

        private async Task<IList<PictureparkAsset>> TransformAssetsAsync(IList<SmintIoAsset> assets, string transferIdentifier)
        {
            IList<PictureparkAsset> targetAssets = new List<PictureparkAsset>();

            foreach (var asset in assets)
            {
                _logger.LogInformation($"Transforming Smint.io LPT {asset.LicensePurchaseTransactionUuid}...");

                var binaries = asset.Binaries;

                IList<PictureparkAsset> assetTargetAssets = new List<PictureparkAsset>();

                foreach (var binary in binaries)
                {
                    var binaryUuid = binary.Uuid;
                    var binaryVersion = binary.Version;

                    var downloadUrl = binary.DownloadUrl;
                    var recommendedFileName = binary.RecommendedFileName;

                    var name = binary.Name?.Count > 0 ? binary.Name : asset.Name;
                    var usage = binary.Usage;

                    var targetAsset = new PictureparkAsset()
                    {
                        TransferId = transferIdentifier,
                        LPTUuid = asset.LicensePurchaseTransactionUuid,
                        BinaryUuid = binaryUuid,
                        BinaryVersion = binaryVersion,
                        FindAgainFileUuid = $"{asset.LicensePurchaseTransactionUuid}_{binaryUuid}",
                        IsCompoundAsset = false,
                        RecommendedFileName = recommendedFileName,
                        DownloadUrl = downloadUrl,
                        Name = name,
                        Usage = usage
                    };

                    targetAsset.Metadata = new DataDictionary()
                    {
                        { nameof(SmintIoContentLayer), await GetContentMetadataAsync(asset, binary) },
                        { nameof(SmintIoLicenseLayer), await GetLicenseMetadataAsync(asset) }
                    };

                    assetTargetAssets.Add(targetAsset);

                    targetAssets.Add(targetAsset);
                }

                if (assetTargetAssets.Count > 1)
                {
                    // we have a compound asset

                    var targetCompoundAsset = new PictureparkAsset()
                    {
                        TransferId = transferIdentifier,
                        LPTUuid = asset.LicensePurchaseTransactionUuid,
                        IsCompoundAsset = true,
                        Name = asset.Name,
                        AssetParts = assetTargetAssets
                    };

                    targetCompoundAsset.Metadata = new DataDictionary()
                    {
                        { nameof(SmintIoContentLayer), await GetContentMetadataAsync(asset, null) },
                        { nameof(SmintIoLicenseLayer), await GetLicenseMetadataAsync(asset) }
                    };

                    targetAssets.Add(targetCompoundAsset);
                }

                _logger.LogInformation($"Transformed Smint.io LPT {asset.LicensePurchaseTransactionUuid}");
            }

            return targetAssets;
        }

        private async Task<DataDictionary> GetContentMetadataAsync(SmintIoAsset asset, SmintIoBinary binary)
        {
            var keywords = JoinValues(asset.Keywords);

            var contentProvider = await GetContentProviderPictureparkKeyAsync(asset.Provider);

            var contentTypeString = !string.IsNullOrEmpty(binary?.ContentType) ? binary.ContentType : asset.ContentType;
            var contentType = await GetContentTypePictureparkKeyAsync(contentTypeString);

            var contentCategoryString = !string.IsNullOrEmpty(binary?.Category) ? binary.Category : asset.Category;
            var contentCategory = await GetContentCategoryPictureparkKeyAsync(contentCategoryString);

            var dataDictionary = new DataDictionary
            {
                { "contentProvider", new { _refId = contentProvider } },
                { "contentType", new { _refId = contentType } },
                { "category", new { _refId = contentCategory } },
                { "smintIoUrl", asset.SmintIoUrl },
                { "purchasedAt", asset.PurchasedAt },
                { "createdAt", asset.CreatedAt },
                { "licensePurchaseTransactionUuid", asset.LicensePurchaseTransactionUuid },
                { "cartPurchaseTransactionUuid", asset.CartPurchaseTransactionUuid },
                { "hasBeenCancelled", asset.State == SmintIo.CLAPI.Consumer.Client.Generated.LicensePurchaseTransactionStateEnum.Cancelled }
            };

            if (!string.IsNullOrEmpty(binary?.BinaryType))
            {
                var binaryType = await GetBinaryTypePictureparkKeyAsync(binary.BinaryType);

                dataDictionary.Add("binaryType", new { _refId = binaryType });
            }

            if (binary?.Name?.Count > 0)
                dataDictionary.Add("name", binary.Name);
            else if (asset.Name?.Count > 0)
                dataDictionary.Add("name", asset.Name);

            if (binary?.Name?.Count > 0)
                dataDictionary.Add("description", binary.Description);
            else if (asset.Description?.Count > 0)
                dataDictionary.Add("description", asset.Description);

            if (!string.IsNullOrEmpty(asset.ProjectUuid))
                dataDictionary.Add("projectUuid", asset.ProjectUuid);

            if (asset.ProjectName?.Count > 0)
                dataDictionary.Add("projectName", asset.ProjectName);

            if (!string.IsNullOrEmpty(asset.CollectionUuid))
                dataDictionary.Add("collectionUuid", asset.CollectionUuid);

            if (asset.CollectionName?.Count > 0)
                dataDictionary.Add("collectionName", asset.CollectionName);

            if (asset.Keywords?.Count > 0)
                dataDictionary.Add("keywords", keywords);

            if (asset.CopyrightNotices?.Count > 0)
                dataDictionary.Add("copyrightNotices", asset.CopyrightNotices);

            if (binary != null)
            {
                dataDictionary.Add("binaryUuid", binary.Uuid);

                var binaryCulture = binary.Culture;
                if (!string.IsNullOrEmpty(binaryCulture))
                    dataDictionary.Add("binaryCulture", binaryCulture);

                dataDictionary.Add("binaryVersion", binary.Version);
            }

            if (asset.IsEditorialUse != null)
                dataDictionary.Add("isEditorialUse", asset.IsEditorialUse);

            if (asset.HasLicenseUsageConstraints != null)
                dataDictionary.Add("hasLicenseUsageConstraints", asset.HasLicenseUsageConstraints);

            return dataDictionary;
        }

        private async Task<string> GetContentProviderPictureparkKeyAsync(string smintIoKey)
        {
            var contentProviderListItems = await _pictureparkClient.GetContentProvidersAsync();

            return contentProviderListItems.First(contentProviderListItem => string.Equals(contentProviderListItem.SmintIoKey, smintIoKey)).PictureparkListItemId;
        }

        private async Task<string> GetContentTypePictureparkKeyAsync(string smintIoKey)
        {
            var contentTypeListItems = await _pictureparkClient.GetContentTypesAsync();

            return contentTypeListItems.First(contentTypeListItem => string.Equals(contentTypeListItem.SmintIoKey, smintIoKey)).PictureparkListItemId;
        }

        private async Task<string> GetBinaryTypePictureparkKeyAsync(string smintIoKey)
        {
            var binaryTypeListItems = await _pictureparkClient.GetBinaryTypesAsync();

            return binaryTypeListItems.First(binaryTypeListItem => string.Equals(binaryTypeListItem.SmintIoKey, smintIoKey)).PictureparkListItemId;
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
                { "licenseeName", asset.LicenseeName },
                { "licenseeUuid", asset.LicenseeUuid },
                { "licenseType", new { _refId = licenseType }},
                { "hasBeenCancelled", asset.State == SmintIo.CLAPI.Consumer.Client.Generated.LicensePurchaseTransactionStateEnum.Cancelled }
            };

            if (asset.LicenseText?.Count > 0)
                dataDictionary.Add("licenseText", asset.LicenseText);

            if (asset.LicenseOptions?.Count > 0)
                dataDictionary.Add("licenseOptions", GetLicenseOptions(asset.LicenseOptions));

            if (asset.UsageConstraints?.Count > 0)
                dataDictionary.Add("usageConstraints", await GetUsageConstraintsAsync(asset.UsageConstraints));

            if (asset.DownloadConstraints != null)
                dataDictionary.Add("downloadConstraints", GetDownloadConstraints(asset.DownloadConstraints));

            if (asset.ReleaseDetails != null)
                dataDictionary.Add("releaseDetails", await GetReleaseDetailsMetadataAsync(asset.ReleaseDetails));

            if (asset.IsEditorialUse != null)
                dataDictionary.Add("isEditorialUse", asset.IsEditorialUse);

            if (asset.HasLicenseUsageConstraints != null)
                dataDictionary.Add("hasLicenseUsageConstraints", asset.HasLicenseUsageConstraints);

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

        private async Task<DataDictionary[]> GetUsageConstraintsAsync(IList<SmintIo.CLAPI.Consumer.Integration.Core.Contracts.SmintIoUsageConstraints> usageContraints)
        {
            var dataDictionaries = new List<DataDictionary>();

            foreach (var usageConstraint in usageContraints)
            {
                var exclusivities = await GetLicenseExclusivitiesPictureparkKeysAsync(usageConstraint.Exclusivities);

                var allowedUsages = await GetLicenseUsagesPictureparkKeysAsync(usageConstraint.AllowedUsages);
                var restrictedUsages = await GetLicenseUsagesPictureparkKeysAsync(usageConstraint.RestrictedUsages);

                var allowedSizes = await GetLicenseSizesPictureparkKeysAsync(usageConstraint.AllowedSizes);
                var restrictedSizes = await GetLicenseSizesPictureparkKeysAsync(usageConstraint.RestrictedSizes);

                var allowedPlacements = await GetLicensePlacementsPictureparkKeysAsync(usageConstraint.AllowedPlacements);
                var restrictedPlacements = await GetLicensePlacementsPictureparkKeysAsync(usageConstraint.RestrictedPlacements);

                var allowedDistributions = await GetLicenseDistributionsPictureparkKeysAsync(usageConstraint.AllowedDistributions);
                var restrictedDistributions = await GetLicenseDistributionsPictureparkKeysAsync(usageConstraint.RestrictedDistributions);

                var allowedGeographies = await GetLicenseGeographiesPictureparkKeysAsync(usageConstraint.AllowedGeographies);
                var restrictedGeographies = await GetLicenseGeographiesPictureparkKeysAsync(usageConstraint.RestrictedGeographies);

                var allowedVerticals = await GetLicenseVerticalsPictureparkKeysAsync(usageConstraint.AllowedVerticals);
                var restrictedVerticals = await GetLicenseVerticalsPictureparkKeysAsync(usageConstraint.RestrictedVerticals);

                var allowedLanguages = await GetLicenseLanguagesPictureparkKeysAsync(usageConstraint.AllowedLanguages);
                var restrictedLanguages = await GetLicenseLanguagesPictureparkKeysAsync(usageConstraint.RestrictedLanguages);

                var dataDictionary = new DataDictionary()
                {
                    { "validFrom", usageConstraint.ValidFrom }
                };

                if (usageConstraint.SequenceNumber != null)
                    dataDictionary.Add("sequenceNumber", usageConstraint.SequenceNumber);

                if (usageConstraint.Name?.Count > 0)
                    dataDictionary.Add("name", usageConstraint.Name);

                if (exclusivities?.Count() > 0)
                    dataDictionary.Add("exclusivities", exclusivities.Select(exclusivity => new { _refId = exclusivity }).ToArray());

                if (allowedUsages?.Count() > 0)
                    dataDictionary.Add("allowedUsages", allowedUsages.Select(usage => new { _refId = usage }).ToArray());

                if (restrictedUsages?.Count() > 0)
                    dataDictionary.Add("restrictedUsages", restrictedUsages.Select(usage => new { _refId = usage }).ToArray());

                if (allowedSizes?.Count() > 0)
                    dataDictionary.Add("allowedSizes", allowedSizes.Select(size => new { _refId = size }).ToArray());

                if (restrictedSizes?.Count() > 0)
                    dataDictionary.Add("restrictedSizes", restrictedSizes.Select(size => new { _refId = size }).ToArray());

                if (allowedPlacements?.Count() > 0)
                    dataDictionary.Add("allowedPlacements", allowedPlacements.Select(placement => new { _refId = placement }).ToArray());

                if (restrictedPlacements?.Count() > 0)
                    dataDictionary.Add("restrictedPlacements", restrictedPlacements.Select(placement => new { _refId = placement }).ToArray());

                if (allowedDistributions?.Count() > 0)
                    dataDictionary.Add("allowedDistributions", allowedDistributions.Select(distribution => new { _refId = distribution }).ToArray());

                if (restrictedDistributions?.Count() > 0)
                    dataDictionary.Add("restrictedDistributions", restrictedDistributions.Select(distribution => new { _refId = distribution }).ToArray());

                if (allowedGeographies?.Count() > 0)
                    dataDictionary.Add("allowedGeographies", allowedGeographies.Select(geography => new { _refId = geography }).ToArray());

                if (restrictedGeographies?.Count() > 0)
                    dataDictionary.Add("restrictedGeographies", restrictedGeographies.Select(geography => new { _refId = geography }).ToArray());

                if (allowedVerticals?.Count() > 0)
                    dataDictionary.Add("allowedVerticals", allowedVerticals.Select(vertical => new { _refId = vertical }).ToArray());

                if (restrictedVerticals?.Count() > 0)
                    dataDictionary.Add("restrictedVerticals", restrictedVerticals.Select(vertical => new { _refId = vertical }).ToArray());

                if (allowedLanguages?.Count() > 0)
                    dataDictionary.Add("allowedLanguages", allowedLanguages.Select(language => new { _refId = language }).ToArray());

                if (restrictedLanguages?.Count() > 0)
                    dataDictionary.Add("restrictedLanguages", restrictedLanguages.Select(language => new { _refId = language }).ToArray());

                if (usageConstraint.MaxEditions != null)
                    dataDictionary.Add("maxEditions", usageConstraint.MaxEditions);

                if (usageConstraint.ValidUntil != null)
                    dataDictionary.Add("validUntil", usageConstraint.ValidUntil);

                if (usageConstraint.ToBeUsedUntil != null)
                    dataDictionary.Add("toBeUsedUntil", usageConstraint.ToBeUsedUntil);

                if (usageConstraint.IsEditorialUse != null)
                    dataDictionary.Add("isEditorialUse", usageConstraint.IsEditorialUse);

                dataDictionaries.Add(dataDictionary);
            }

            return dataDictionaries.ToArray();
        }

        private async Task<IList<string>> GetLicenseExclusivitiesPictureparkKeysAsync(IList<string> smintIoKeys)
        {
            if (smintIoKeys == null || !smintIoKeys.Any())
                return null;

            var licenseExclusivitiesListItems = await _pictureparkClient.GetLicenseExclusivitiesAsync();

            return licenseExclusivitiesListItems
                .Where(licenseExclusivityListItem => smintIoKeys.Any(smintIoKey => string.Equals(licenseExclusivityListItem.SmintIoKey, smintIoKey)))
                .Select(licenseExclusivityListItem => licenseExclusivityListItem.PictureparkListItemId)
                .ToList();
        }

        private async Task<IList<string>> GetLicenseUsagesPictureparkKeysAsync(IList<string> smintIoKeys)
        {
            if (smintIoKeys == null || !smintIoKeys.Any())
                return null;

            var licenseUsagesListItems = await _pictureparkClient.GetLicenseUsagesAsync();

            return licenseUsagesListItems
                .Where(licenseUsageListItem => smintIoKeys.Any(smintIoKey => string.Equals(licenseUsageListItem.SmintIoKey, smintIoKey)))
                .Select(licenseUsageListItem => licenseUsageListItem.PictureparkListItemId)
                .ToList();
        }

        private async Task<IList<string>> GetLicenseSizesPictureparkKeysAsync(IList<string> smintIoKeys)
        {
            if (smintIoKeys == null || !smintIoKeys.Any())
                return null;

            var licenseSizesListItems = await _pictureparkClient.GetLicenseSizesAsync();

            return licenseSizesListItems
                .Where(licenseSizeListItem => smintIoKeys.Any(smintIoKey => string.Equals(licenseSizeListItem.SmintIoKey, smintIoKey)))
                .Select(licenseSizeListItem => licenseSizeListItem.PictureparkListItemId)
                .ToList();
        }

        private async Task<IList<string>> GetLicensePlacementsPictureparkKeysAsync(IList<string> smintIoKeys)
        {
            if (smintIoKeys == null || !smintIoKeys.Any())
                return null;

            var licensePlacementsListItems = await _pictureparkClient.GetLicensePlacementsAsync();

            return licensePlacementsListItems
                .Where(licensePlacementListItem => smintIoKeys.Any(smintIoKey => string.Equals(licensePlacementListItem.SmintIoKey, smintIoKey)))
                .Select(licensePlacementListItem => licensePlacementListItem.PictureparkListItemId)
                .ToList();
        }

        private async Task<IList<string>> GetLicenseDistributionsPictureparkKeysAsync(IList<string> smintIoKeys)
        {
            if (smintIoKeys == null || !smintIoKeys.Any())
                return null;

            var licenseDistributionsListItems = await _pictureparkClient.GetLicenseDistributionsAsync();

            return licenseDistributionsListItems
                .Where(licenseDistributionListItem => smintIoKeys.Any(smintIoKey => string.Equals(licenseDistributionListItem.SmintIoKey, smintIoKey)))
                .Select(licenseDistributionListItem => licenseDistributionListItem.PictureparkListItemId)
                .ToList();
        }

        private async Task<IList<string>> GetLicenseGeographiesPictureparkKeysAsync(IList<string> smintIoKeys)
        {
            if (smintIoKeys == null || !smintIoKeys.Any())
                return null;

            var licenseGeographiesListItems = await _pictureparkClient.GetLicenseGeographiesAsync();

            return licenseGeographiesListItems
                .Where(licenseGeographyListItem => smintIoKeys.Any(smintIoKey => string.Equals(licenseGeographyListItem.SmintIoKey, smintIoKey)))
                .Select(licenseGeographyListItem => licenseGeographyListItem.PictureparkListItemId)
                .ToList();
        }

        private async Task<IList<string>> GetLicenseVerticalsPictureparkKeysAsync(IList<string> smintIoKeys)
        {
            if (smintIoKeys == null || !smintIoKeys.Any())
                return null;

            var licenseVerticalsListItems = await _pictureparkClient.GetLicenseVerticalsAsync();

            return licenseVerticalsListItems
                .Where(licenseVerticalListItem => smintIoKeys.Any(smintIoKey => string.Equals(licenseVerticalListItem.SmintIoKey, smintIoKey)))
                .Select(licenseVerticalListItem => licenseVerticalListItem.PictureparkListItemId)
                .ToList();
        }

        private async Task<IList<string>> GetLicenseLanguagesPictureparkKeysAsync(IList<string> smintIoKeys)
        {
            if (smintIoKeys == null || !smintIoKeys.Any())
                return null;

            var licenseLanguagesListItems = await _pictureparkClient.GetLicenseLanguagesAsync();

            return licenseLanguagesListItems
                .Where(licenseLanguageListItem => smintIoKeys.Any(smintIoKey => string.Equals(licenseLanguageListItem.SmintIoKey, smintIoKey)))
                .Select(licenseLanguageListItem => licenseLanguageListItem.PictureparkListItemId)
                .ToList();
        }

        private DataDictionary GetDownloadConstraints(SmintIo.CLAPI.Consumer.Integration.Core.Contracts.SmintIoDownloadConstraints downloadConstraints)
        {
            if (downloadConstraints == null)
            {
                return null;
            }

            var dataDictionary = new DataDictionary();

            if (downloadConstraints.MaxDownloads != null)
                dataDictionary.Add("maxDownloads", downloadConstraints.MaxDownloads);

            if (downloadConstraints.MaxUsers != null)
                dataDictionary.Add("maxUsers", downloadConstraints.MaxUsers);

            if (downloadConstraints.MaxReuses != null)
                dataDictionary.Add("maxReuses", downloadConstraints.MaxReuses);

            return dataDictionary;
        }

        private async Task<DataDictionary> GetReleaseDetailsMetadataAsync(SmintIo.CLAPI.Consumer.Integration.Core.Contracts.SmintIoReleaseDetails releaseDetails)
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

#pragma warning disable CS1998 // Bei der asynchronen Methode fehlen "await"-Operatoren. Die Methode wird synchron ausgeführt.
        public async Task HandleAuthenticatorExceptionAsync(SmintIoAuthenticatorException exception)
#pragma warning restore CS1998 // Bei der asynchronen Methode fehlen "await"-Operatoren. Die Methode wird synchron ausgeführt.
        {
            _logger.LogError(exception, "Error in Smint.io authenticator");
        }

#pragma warning disable CS1998 // Bei der asynchronen Methode fehlen "await"-Operatoren. Die Methode wird synchron ausgeführt.
        public async Task HandleSyncJobExceptionAsync(SmintIoSyncJobException exception)
#pragma warning restore CS1998 // Bei der asynchronen Methode fehlen "await"-Operatoren. Die Methode wird synchron ausgeführt.
        {
            _logger.LogError(exception, "Error in Smint.io sync job");
        }

        public void ClearCaches()
        {
            _pictureparkClient.ClearCache();
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
    }
}
