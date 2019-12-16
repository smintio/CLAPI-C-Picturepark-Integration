using System.Collections.Generic;
using System.Threading.Tasks;
using Client.Contracts;

namespace Client.Providers
{
    public interface IPictureparkApiClientProvider
    {
        Task InitSchemasAsync();

        Task<IList<PictureparkListItem>> GetContentProvidersAsync();
        Task ImportContentProvidersAsync(IList<PictureparkListItem> contentProviders);

        Task<IList<PictureparkListItem>> GetContentTypesAsync();
        Task ImportContentTypesAsync(IList<PictureparkListItem> contentTypes);

        Task<IList<PictureparkListItem>> GetBinaryTypesAsync();
        Task ImportBinaryTypesAsync(IList<PictureparkListItem> binaryTypes);

        Task<IList<PictureparkListItem>> GetContentCategoriesAsync();
        Task ImportContentCategoriesAsync(IList<PictureparkListItem> contentCategories);

        Task<IList<PictureparkListItem>> GetLicenseTypesAsync();
        Task ImportLicenseTypesAsync(IList<PictureparkListItem> licenseTypes);

        Task<IList<PictureparkListItem>> GetReleaseStatesAsync();
        Task ImportReleaseStatesAsync(IList<PictureparkListItem> releaseStates);

        Task<IList<PictureparkListItem>> GetLicenseExclusivitiesAsync();
        Task ImportLicenseExclusivitiesAsync(IList<PictureparkListItem> licenseExclusivities);

        Task<IList<PictureparkListItem>> GetLicenseUsagesAsync();
        Task ImportLicenseUsagesAsync(IList<PictureparkListItem> licenseUsages);

        Task<IList<PictureparkListItem>> GetLicenseSizesAsync();
        Task ImportLicenseSizesAsync(IList<PictureparkListItem> licenseSizes);

        Task<IList<PictureparkListItem>> GetLicensePlacementsAsync();
        Task ImportLicensePlacementsAsync(IList<PictureparkListItem> licensePlacements);

        Task<IList<PictureparkListItem>> GetLicenseDistributionsAsync();
        Task ImportLicenseDistributionsAsync(IList<PictureparkListItem> licenseDistributions);

        Task<IList<PictureparkListItem>> GetLicenseGeographiesAsync();
        Task ImportLicenseGeographiesAsync(IList<PictureparkListItem> licenseGeographies);

        Task<IList<PictureparkListItem>> GetLicenseIndustriesAsync();
        Task ImportLicenseIndustriesAsync(IList<PictureparkListItem> licenseIndustries);

        Task<IList<PictureparkListItem>> GetLicenseLanguagesAsync();
        Task ImportLicenseLanguagesAsync(IList<PictureparkListItem> licenseLanguages);

        Task<IList<PictureparkListItem>> GetLicenseUsageLimitsAsync();
        Task ImportLicenseUsageLimitsAsync(IList<PictureparkListItem> licenseUsageLimits);

        Task<string> GetExistingPictureparkCompoundAssetUuid(string smintIoCompoundAssetUuid);
        Task<string> GetExistingPictureparkAssetBinaryUuid(string smintIoAssetUuid, string smintIoBinaryUuid);

        Task CreateAssetsAsync(IList<PictureparkAsset> newTargetAssets);
        Task UpdateAssetsAsync(IList<PictureparkAsset> updatedTargetAssets);

        Task CreateCompoundAssetsAsync(IList<PictureparkAsset> newTargetCompoundAssets);
        Task UpdateCompoundAssetsAsync(IList<PictureparkAsset> updatedTargetCompoundAssets);
    }
}
