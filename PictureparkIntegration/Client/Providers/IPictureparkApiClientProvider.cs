using System.Collections.Generic;
using System.Threading.Tasks;
using Client.Contracts;

namespace Client.Providers
{
    public interface IPictureparkApiClientProvider
    {
        Task InitSchemasAsync();

        void ClearCache();

        Task<IList<PictureparkListItem>> GetContentProvidersAsync();
        Task ImportContentProvidersAsync(IList<PictureparkListItem> contentProviders);

        Task<IList<PictureparkListItem>> GetContentCategoriesAsync();
        Task ImportContentCategoriesAsync(IList<PictureparkListItem> contentCategories);

        Task<IList<PictureparkListItem>> GetLicenseTypesAsync();
        Task ImportLicenseTypesAsync(IList<PictureparkListItem> licenseTypes);

        Task<IList<PictureparkListItem>> GetReleaseStatesAsync();
        Task ImportReleaseStatesAsync(IList<PictureparkListItem> releaseStates);

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

        Task<IList<PictureparkListItem>> GetLicenseVerticalsAsync();
        Task ImportLicenseVerticalsAsync(IList<PictureparkListItem> licenseVerticals);

        Task ImportAssetsAsync(string folderName, IList<PictureparkAsset> assets);
    }
}
