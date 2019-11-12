using Client.Contracts;
using Client.Providers;
using Microsoft.Extensions.Logging;
using Picturepark.SDK.V1.Contract;
using SmintIo.CLAPI.Consumer.Integration.Core.Contracts;
using SmintIo.CLAPI.Consumer.Integration.Core.Exceptions;
using SmintIo.CLAPI.Consumer.Integration.Core.Target;
using SmintIo.CLAPI.Consumer.Integration.Core.Target.Impl;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Client.Target.Impl
{
    public class PictureparkSyncTargetImpl : ISyncTarget<PictureparkAsset, PictureparkLicenseOption, PictureparkLicenseTerm, PictureparkReleaseDetails, PictureparkDownloadConstraints>
    {
        private static readonly ISyncTargetCapabilities Capabilities = new SyncTargetCapabilitiesImpl(
            SyncTargetCapabilitiesEnum.MultiLanguageEnum,
            SyncTargetCapabilitiesEnum.CompoundAssetsEnum,
            SyncTargetCapabilitiesEnum.BinaryUpdatesEnum);

        private readonly IPictureparkApiClientProvider _pictureparkClient;

        private readonly ILogger<PictureparkSyncTargetImpl> _logger;

        public PictureparkSyncTargetImpl(
            IPictureparkApiClientProvider pictureparkClient,
            ILogger<PictureparkSyncTargetImpl> logger)
        {
            _pictureparkClient = pictureparkClient;

            _logger = logger;
        }

        public ISyncTargetCapabilities GetCapabilities()
        {
            return Capabilities;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<bool> BeforeSyncAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            return true;
        }


#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<bool> BeforeGenericMetadataSyncAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            return true;
        }

        public async Task ImportContentProvidersAsync(IList<SmintIoMetadataElement> contentProviders)
        {
            var transformedContentProviders = TransformGenericMetadata(contentProviders);
            await _pictureparkClient.ImportContentProvidersAsync(transformedContentProviders);
        }

        public async Task<string> GetContentProviderKeyAsync(string contentProvider)
        {
            var contentProviderListItems = await _pictureparkClient.GetContentProvidersAsync();

            return contentProviderListItems.First(contentProviderListItem => string.Equals(contentProviderListItem.SmintIoKey, contentProvider)).PictureparkListItemId;
        }

        public async Task ImportContentTypesAsync(IList<SmintIoMetadataElement> contentTypes)
        {
            var transformedContentTypes = TransformGenericMetadata(contentTypes);
            await _pictureparkClient.ImportContentTypesAsync(transformedContentTypes);
        }

        public async Task<string> GetContentTypeKeyAsync(string contentType)
        {
            var contentTypeListItems = await _pictureparkClient.GetContentTypesAsync();

            return contentTypeListItems.First(contentTypeListItem => string.Equals(contentTypeListItem.SmintIoKey, contentType)).PictureparkListItemId;
        }

        public async Task ImportBinaryTypesAsync(IList<SmintIoMetadataElement> binaryTypes)
        {
            var transformedBinaryTypes = TransformGenericMetadata(binaryTypes);
            await _pictureparkClient.ImportBinaryTypesAsync(transformedBinaryTypes);
        }

        public async Task<string> GetBinaryTypeKeyAsync(string binaryType)
        {
            var binaryTypeListItems = await _pictureparkClient.GetBinaryTypesAsync();

            return binaryTypeListItems.First(binaryTypeListItem => string.Equals(binaryTypeListItem.SmintIoKey, binaryType)).PictureparkListItemId;
        }

        public async Task ImportContentCategoriesAsync(IList<SmintIoMetadataElement> contentCategories)
        {
            var transformedContentCategories = TransformGenericMetadata(contentCategories);
            await _pictureparkClient.ImportContentCategoriesAsync(transformedContentCategories);
        }

        public async Task<string> GetContentCategoryKeyAsync(string contentCategory)
        {
            var contentCategoryListItems = await _pictureparkClient.GetContentCategoriesAsync();

            return contentCategoryListItems.First(contentCategoryListItem => string.Equals(contentCategoryListItem.SmintIoKey, contentCategory)).PictureparkListItemId;
        }

        public async Task ImportLicenseTypesAsync(IList<SmintIoMetadataElement> licenseTypes)
        {
            var transformedLicenseTypes = TransformGenericMetadata(licenseTypes);
            await _pictureparkClient.ImportLicenseTypesAsync(transformedLicenseTypes);
        }

        public async Task<string> GetLicenseTypeKeyAsync(string licenseType)
        {
            var licenseTypeListItems = await _pictureparkClient.GetLicenseTypesAsync();

            return licenseTypeListItems.First(licenseTypeListItem => string.Equals(licenseTypeListItem.SmintIoKey, licenseType)).PictureparkListItemId;
        }

        public async Task ImportReleaseStatesAsync(IList<SmintIoMetadataElement> releaseStates)
        {
            var transformedReleaseStates = TransformGenericMetadata(releaseStates);
            await _pictureparkClient.ImportReleaseStatesAsync(transformedReleaseStates);
        }

        public async Task<string> GetReleaseStateKeyAsync(string releaseState)
        {
            var releaseStateListItems = await _pictureparkClient.GetReleaseStatesAsync();

            return releaseStateListItems.First(releaseStateListItem => string.Equals(releaseStateListItem.SmintIoKey, releaseState)).PictureparkListItemId;
        }

        public async Task ImportLicenseExclusivitiesAsync(IList<SmintIoMetadataElement> licenseExclusivities)
        {
            var transformedLicenseExclusivities = TransformGenericMetadata(licenseExclusivities);
            await _pictureparkClient.ImportLicenseExclusivitiesAsync(transformedLicenseExclusivities);
        }

        public async Task<string> GetLicenseExclusivityKeyAsync(string licenseExclusivity)
        {
            var licenseExclusivityListItems = await _pictureparkClient.GetLicenseExclusivitiesAsync();

            return licenseExclusivityListItems.First(licenseExclusivityListItem => string.Equals(licenseExclusivityListItem.SmintIoKey, licenseExclusivity)).PictureparkListItemId;
        }

        public async Task ImportLicenseUsagesAsync(IList<SmintIoMetadataElement> licenseUsages)
        {
            var transformedLicenseUsages = TransformGenericMetadata(licenseUsages);
            await _pictureparkClient.ImportLicenseUsagesAsync(transformedLicenseUsages);
        }

        public async Task<string> GetLicenseUsageKeyAsync(string licenseUsage)
        {
            var licenseUsageListItems = await _pictureparkClient.GetLicenseUsagesAsync();

            return licenseUsageListItems.First(licenseUsageListItem => string.Equals(licenseUsageListItem.SmintIoKey, licenseUsage)).PictureparkListItemId;
        }

        public async Task ImportLicenseSizesAsync(IList<SmintIoMetadataElement> licenseSizes)
        {
            var transformedLicenseSizes = TransformGenericMetadata(licenseSizes);
            await _pictureparkClient.ImportLicenseSizesAsync(transformedLicenseSizes);
        }

        public async Task<string> GetLicenseSizeKeyAsync(string licenseSize)
        {
            var licenseSizeListItems = await _pictureparkClient.GetLicenseSizesAsync();

            return licenseSizeListItems.First(licenseSizeListItem => string.Equals(licenseSizeListItem.SmintIoKey, licenseSize)).PictureparkListItemId;
        }

        public async Task ImportLicensePlacementsAsync(IList<SmintIoMetadataElement> licensePlacements)
        {
            var transformedLicensePlacements = TransformGenericMetadata(licensePlacements);
            await _pictureparkClient.ImportLicensePlacementsAsync(transformedLicensePlacements);
        }

        public async Task<string> GetLicensePlacementKeyAsync(string licensePlacement)
        {
            var licensePlacementListItems = await _pictureparkClient.GetLicensePlacementsAsync();

            return licensePlacementListItems.First(licensePlacementListItem => string.Equals(licensePlacementListItem.SmintIoKey, licensePlacement)).PictureparkListItemId;
        }

        public async Task ImportLicenseDistributionsAsync(IList<SmintIoMetadataElement> licenseDistributions)
        {
            var transformedLicenseDistributions = TransformGenericMetadata(licenseDistributions);
            await _pictureparkClient.ImportLicenseDistributionsAsync(transformedLicenseDistributions);
        }

        public async Task<string> GetLicenseDistributionKeyAsync(string licenseDistribution)
        {
            var licenseDistributionListItems = await _pictureparkClient.GetLicenseDistributionsAsync();

            return licenseDistributionListItems.First(licenseDistributionListItem => string.Equals(licenseDistributionListItem.SmintIoKey, licenseDistribution)).PictureparkListItemId;
        }

        public async Task ImportLicenseGeographiesAsync(IList<SmintIoMetadataElement> licenseGeographies)
        {
            var transformedLicenseGeographies = TransformGenericMetadata(licenseGeographies);
            await _pictureparkClient.ImportLicenseGeographiesAsync(transformedLicenseGeographies);
        }

        public async Task<string> GetLicenseGeographyKeyAsync(string licenseGeography)
        {
            var licenseGeographyListItems = await _pictureparkClient.GetLicenseGeographiesAsync();

            return licenseGeographyListItems.First(licenseGeographyListItem => string.Equals(licenseGeographyListItem.SmintIoKey, licenseGeography)).PictureparkListItemId;
        }

        public async Task ImportLicenseIndustriesAsync(IList<SmintIoMetadataElement> licenseIndustries)
        {
            var transformedLicenseIndustries = TransformGenericMetadata(licenseIndustries);
            await _pictureparkClient.ImportLicenseIndustriesAsync(transformedLicenseIndustries);
        }

        public async Task<string> GetLicenseIndustryKeyAsync(string licenseIndustry)
        {
            var licenseIndustryListItems = await _pictureparkClient.GetLicenseIndustriesAsync();

            return licenseIndustryListItems.First(licenseIndustryListItem => string.Equals(licenseIndustryListItem.SmintIoKey, licenseIndustry)).PictureparkListItemId;
        }

        public async Task ImportLicenseLanguagesAsync(IList<SmintIoMetadataElement> licenseLanguages)
        {
            var transformedLicenseLanguages = TransformGenericMetadata(licenseLanguages);
            await _pictureparkClient.ImportLicenseLanguagesAsync(transformedLicenseLanguages);
        }

        public async Task<string> GetLicenseLanguageKeyAsync(string licenseLanguage)
        {
            var licenseLanguageListItems = await _pictureparkClient.GetLicenseLanguagesAsync();

            return licenseLanguageListItems.First(licenseLanguageListItem => string.Equals(licenseLanguageListItem.SmintIoKey, licenseLanguage)).PictureparkListItemId;
        }

        public async Task ImportLicenseUsageLimitsAsync(IList<SmintIoMetadataElement> licenseUsageLimits)
        {
            var transformedLicenseUsageLimits = TransformGenericMetadata(licenseUsageLimits);
            await _pictureparkClient.ImportLicenseUsageLimitsAsync(transformedLicenseUsageLimits);
        }

        public async Task<string> GetLicenseUsageLimitKeyAsync(string licenseUsageLimit)
        {
            var licenseUsageLimitListItems = await _pictureparkClient.GetLicenseUsageLimitsAsync();

            return licenseUsageLimitListItems.First(licenseUsageLimitListItem => string.Equals(licenseUsageLimitListItem.SmintIoKey, licenseUsageLimit)).PictureparkListItemId;
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


#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task AfterGenericMetadataSyncAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task<bool> BeforeAssetsSyncAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            return true;
        }

        public PictureparkAsset CreateSyncAsset()
        {
            return new PictureparkAsset();
        }

        public PictureparkLicenseOption CreateSyncLicenseOption()
        {
            return new PictureparkLicenseOption();
        }

        public PictureparkLicenseTerm CreateSyncLicenseTerm()
        {
            return new PictureparkLicenseTerm();
        }

        public PictureparkReleaseDetails CreateSyncReleaseDetails()
        {
            return new PictureparkReleaseDetails();
        }

        public PictureparkDownloadConstraints CreateSyncDownloadConstraints()
        {
            return new PictureparkDownloadConstraints();
        }

        public async Task<string> GetTargetAssetUuidAsync(string licensePurchaseTransactionUuid, string binaryUuid, bool isCompoundAsset)
        {
            return await _pictureparkClient.GetExistingAssetUuidAsync(licensePurchaseTransactionUuid, binaryUuid, isCompoundAsset);
        }

        public async Task CreateTargetAssetsAsync(string folderName, IList<PictureparkAsset> newTargetAssets)
        {
            await _pictureparkClient.CreateAssetsAsync(folderName, newTargetAssets);
        }

        public async Task UpdateTargetAssetsAsync(string folderName, IList<PictureparkAsset> updatedTargetAssets)
        {
            await _pictureparkClient.UpdateAssetsAsync(folderName, updatedTargetAssets);
        }

        public async Task CreateTargetCompoundAssetsAsync(IList<PictureparkAsset> newTargetCompoundAssets)
        {
            await _pictureparkClient.CreateCompoundAssetsAsync(newTargetCompoundAssets); 
        }

        public async Task UpdateTargetCompoundAssetsAsync(IList<PictureparkAsset> updatedTargetCompoundAssets)
        {
            await _pictureparkClient.UpdateCompoundAssetsAsync(updatedTargetCompoundAssets);
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

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task AfterAssetsSyncAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
        }


#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task AfterSyncAsync()
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
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

        public void ClearGenericMetadataCaches()
        {
            _pictureparkClient.ClearGenericMetadataCache();
        }
    }
}
