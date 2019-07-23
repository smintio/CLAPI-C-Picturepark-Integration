using System.Collections.Generic;
using System.Threading.Tasks;
using Client.Contracts;

namespace Client.Providers
{
    public interface IPictureparkApiClientProvider
    {
        Task InitSchemasAsync();

        void ClearCache();

        Task<IEnumerable<PictureparkListItem>> GetContentProvidersAsync();
        Task ImportContentProvidersAsync(IEnumerable<PictureparkListItem> contentProviders);

        Task<IEnumerable<PictureparkListItem>> GetContentCategoriesAsync();
        Task ImportContentCategoriesAsync(IEnumerable<PictureparkListItem> contentCategories);

        Task<IEnumerable<PictureparkListItem>> GetLicenseTypesAsync();
        Task ImportLicenseTypesAsync(IEnumerable<PictureparkListItem> licenseTypes);

        Task<IEnumerable<PictureparkListItem>> GetReleaseStatesAsync();
        Task ImportReleaseStatesAsync(IEnumerable<PictureparkListItem> releaseStates);

        Task<IEnumerable<PictureparkListItem>> GetLicenseUsagesAsync();
        Task ImportLicenseUsagesAsync(IEnumerable<PictureparkListItem> licenseUsages);

        Task<IEnumerable<PictureparkListItem>> GetLicenseSizesAsync();
        Task ImportLicenseSizesAsync(IEnumerable<PictureparkListItem> licenseSizes);

        Task<IEnumerable<PictureparkListItem>> GetLicensePlacementsAsync();
        Task ImportLicensePlacementsAsync(IEnumerable<PictureparkListItem> licensePlacements);

        Task<IEnumerable<PictureparkListItem>> GetLicenseDistributionsAsync();
        Task ImportLicenseDistributionsAsync(IEnumerable<PictureparkListItem> licenseDistributions);

        Task<IEnumerable<PictureparkListItem>> GetLicenseGeographiesAsync();
        Task ImportLicenseGeographiesAsync(IEnumerable<PictureparkListItem> licenseGeographies);

        Task<IEnumerable<PictureparkListItem>> GetLicenseVerticalsAsync();
        Task ImportLicenseVerticalsAsync(IEnumerable<PictureparkListItem> licenseVerticals);

        Task ImportAssetsAsync(IEnumerable<PictureparkAsset> assets);
    }
}
