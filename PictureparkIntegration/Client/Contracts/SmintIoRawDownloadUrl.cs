using System.Collections.Generic;

namespace Client.Contracts
{
    public class SmintIoRawDownloadUrl
    {
        public string FileUuid { get; set; }

        public string DownloadUrl { get; set; }

        public string RecommendedFileName { get; set; }

        public IDictionary<string, string> Usage { get; set; }
    }
}
