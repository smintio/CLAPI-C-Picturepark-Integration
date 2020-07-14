using SmintIo.CLAPI.Consumer.Integration.Core.Services;
using SmintIo.CLAPI.Consumer.Integration.Core.SyncClient.Impl;

namespace Client.SyncClient.Impl
{
    public class PictureparkSyncClientImpl : BaseSyncClient
    {
        public PictureparkSyncClientImpl(
            ITimedSynchronizerService timedService, IPusherService pusherService
        ) : base(timedService, pusherService)
        {
        }
    }
}
