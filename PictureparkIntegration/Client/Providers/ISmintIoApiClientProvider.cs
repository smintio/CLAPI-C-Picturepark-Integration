using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Client.Contracts;

namespace Client.Providers
{
    public interface ISmintIoApiClientProvider
    {
        Task<IEnumerable<SmintIoAsset>> GetAssetsAsync(DateTimeOffset? minDate, int offset);
    }
}
