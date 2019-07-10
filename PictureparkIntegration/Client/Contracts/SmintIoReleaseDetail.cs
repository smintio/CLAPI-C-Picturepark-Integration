using System.Collections.Generic;

namespace Client.Contracts
{
    public class SmintIoReleaseDetail
    {
        public string ModelReleaseState { get; set; }
        public string PropertyReleaseState { get; set; }

        public IDictionary<string, string> ProviderAllowedUseComment { get; set; }
        public IDictionary<string, string> ProviderReleaseComment { get; set; }

        public IDictionary<string, string> ProviderUsageRestrictions { get; set; }
    }
}
