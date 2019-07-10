using Client.Providers.Impl.Models;

namespace Client.Providers
{
    public interface ISyncDatabaseProvider
    {
        SyncDatabaseModel GetSyncDatabaseModel();
        void SetSyncDatabaseModel(SyncDatabaseModel syncDatabaseModel);
    }
}
