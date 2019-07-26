﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Client.Contracts;

namespace Client.Providers
{
    public interface ISmintIoApiClientProvider
    {
        Task<SmintIoGenericMetadata> GetGenericMetadataAsync();

        Task<(IList<SmintIoAsset>, string)> GetAssetsAsync(string continuationUuid);
    }
}
