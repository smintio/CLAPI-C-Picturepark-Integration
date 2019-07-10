using Client.Providers.Impl.Models;
using Newtonsoft.Json;
using System.IO;

namespace Client.Providers.Impl
{
    public class SyncDatabaseProviderImpl : ISyncDatabaseProvider
    {
        private const string SyncDatabaseFileName = "sync_database.json";

        public SyncDatabaseModel GetSyncDatabaseModel()
        {
            if (!File.Exists(SyncDatabaseFileName))
                return null;

            return JsonConvert.DeserializeObject<SyncDatabaseModel>(File.ReadAllText(SyncDatabaseFileName));
        }

        public void SetSyncDatabaseModel(SyncDatabaseModel syncDatabaseModel)
        {
            File.WriteAllText(SyncDatabaseFileName, JsonConvert.SerializeObject(syncDatabaseModel));
        }
    }
}
