using Newtonsoft.Json;
using SmintIo.CLAPI.Consumer.Integration.Core.Database;
using SmintIo.CLAPI.Consumer.Integration.Core.Database.Models;
using System.IO;
using System.Threading.Tasks;

namespace Client.Providers.Impl
{
    // file based implementation

    public class SyncDatabaseProviderImpl : ISyncDatabaseProvider
    {
        private const string SyncDatabaseFileName = "sync_database.json";

#pragma warning disable CS1998 // Bei der asynchronen Methode fehlen "await"-Operatoren. Die Methode wird synchron ausgeführt.
        public async Task<SyncDatabaseModel> GetSyncDatabaseModelAsync()
#pragma warning restore CS1998 // Bei der asynchronen Methode fehlen "await"-Operatoren. Die Methode wird synchron ausgeführt.
        {
            if (!File.Exists(SyncDatabaseFileName))
            {
                return new SyncDatabaseModel()
                {
                    ContinuationUuid = null
                };
            }

            return JsonConvert.DeserializeObject<SyncDatabaseModel>(File.ReadAllText(SyncDatabaseFileName));
        }

#pragma warning disable CS1998 // Bei der asynchronen Methode fehlen "await"-Operatoren. Die Methode wird synchron ausgeführt.
        public async Task SetSyncDatabaseModelAsync(SyncDatabaseModel syncDatabaseModel)
#pragma warning restore CS1998 // Bei der asynchronen Methode fehlen "await"-Operatoren. Die Methode wird synchron ausgeführt.
        {
            File.WriteAllText(SyncDatabaseFileName, JsonConvert.SerializeObject(syncDatabaseModel));
        }
    }
}
