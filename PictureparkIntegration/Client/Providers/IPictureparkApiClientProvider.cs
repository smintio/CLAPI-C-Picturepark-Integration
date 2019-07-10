using System.Collections.Generic;
using System.Threading.Tasks;
using Client.Contracts;

namespace Client.Providers
{
    public interface IPictureparkApiClientProvider
    {
        Task InitSchemasAsync();
        Task ImportAssetsAsync(IEnumerable<PictureparkAsset> assets);
    }
}
