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
    public class PictureparkSyncTargetImpl : ISyncTarget<PictureparkAsset, PictureparkLicenseTerm, PictureparkReleaseDetails, PictureparkDownloadConstraints>
    {
        private static readonly BaseSyncTargetCapabilities Capabilities = new BaseSyncTargetCapabilities(
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

        public BaseSyncTargetCapabilities GetCapabilities()
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

        public async Task ImportLicenseIndustriesAsync(IList<SmintIoMetadataElement> licenseIndustries)
        {
            var transformedLicenseIndustries = TransformGenericMetadata(licenseIndustries);
            await _pictureparkClient.ImportLicenseIndustriesAsync(transformedLicenseIndustries);
        }

        public async Task ImportLicenseLanguagesAsync(IList<SmintIoMetadataElement> licenseLanguages)
        {
            var transformedLicenseLanguages = TransformGenericMetadata(licenseLanguages);
            await _pictureparkClient.ImportLicenseLanguagesAsync(transformedLicenseLanguages);
        }

        public async Task ImportLicenseUsageLimitsAsync(IList<SmintIoMetadataElement> licenseUsageLimits)
        {
            var transformedLicenseUsageLimits = TransformGenericMetadata(licenseUsageLimits);
            await _pictureparkClient.ImportLicenseUsageLimitsAsync(transformedLicenseUsageLimits);
        }

        private IList<PictureparkListItem> TransformGenericMetadata(IList<SmintIoMetadataElement> smintIoGenericMetadataElements)
        {
            return smintIoGenericMetadataElements.Select(smintIoGenericMetadataElement =>
            {
                return new PictureparkListItem()
                {
                    SmintIoKey = smintIoGenericMetadataElement.Key,
                    SmintIoMetadataElement = smintIoGenericMetadataElement,
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

        public async Task<string> GetTargetCompoundAssetUuidAsync(string smintIoAssetUuid, string recommendedFileName)
        {
            return await _pictureparkClient.GetExistingPictureparkCompoundAssetUuidAsync(smintIoAssetUuid);
        }

        public async Task<string> GetTargetAssetBinaryUuidAsync(string smintIoAssetUuid, string smintIoBinaryUuid, string recommendedFileName)
        {
            return await _pictureparkClient.GetExistingPictureparkAssetBinaryUuidAsync(smintIoAssetUuid, smintIoBinaryUuid);
        }

        public async Task ImportNewTargetAssetsAsync(IList<PictureparkAsset> newTargetAssets)
        {
            await _pictureparkClient.CreateAssetsAsync(newTargetAssets);
        }

        public async Task UpdateTargetAssetsAsync(IList<PictureparkAsset> updatedTargetAssets)
        {
            await _pictureparkClient.UpdateAssetsAsync(updatedTargetAssets);
        }

        public async Task ImportNewTargetCompoundAssetsAsync(IList<PictureparkAsset> newTargetCompoundAssets)
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
        public async Task HandleAuthenticatorExceptionAsync(AuthenticatorException exception)
#pragma warning restore CS1998 // Bei der asynchronen Methode fehlen "await"-Operatoren. Die Methode wird synchron ausgeführt.
        {
            _logger.LogError(exception, "Error in authenticator");
        }

#pragma warning disable CS1998 // Bei der asynchronen Methode fehlen "await"-Operatoren. Die Methode wird synchron ausgeführt.
        public async Task HandleSyncJobExceptionAsync(SyncJobException exception)
#pragma warning restore CS1998 // Bei der asynchronen Methode fehlen "await"-Operatoren. Die Methode wird synchron ausgeführt.
        {
            _logger.LogError(exception, "Error in sync job");
        }
    }
}
