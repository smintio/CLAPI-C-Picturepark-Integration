using System;

namespace Client.Providers.Impl.Models
{
    public class SyncDatabaseModel
    {
        public DateTimeOffset? NextMinDate { get; set; }
        public int NextOffset { get; set; }
    }
}
