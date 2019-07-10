using System.Collections.Generic;

namespace Client.Options
{
    public class PictureparkAppOptions
    {
        public PictureparkAppOptions() { }

        public string ApiBaseUrl { get; set; }

        public string CustomerAlias { get; set; }

        public string[] PictureparkFileTypes { get; set; }

        public Dictionary<string, string> ContentLayerNames { get; set; }
        public Dictionary<string, string> LicenseLayerNames { get; set; }
    }
}
