using System.Collections.Generic;

namespace Client.Contracts
{
    public class SmintIoMetadataElement
    {
        public string Key { get; set; }

        public IDictionary<string, string> Values { get; set; }
    }
}
