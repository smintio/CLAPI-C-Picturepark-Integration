using System.Collections.Generic;

namespace Client.Contracts
{
    public class SmintIoLicenseOptions
    {
        public IDictionary<string, string> OptionName { get; set; }
        public IDictionary<string, string> LicenseText { get; set; }
    }
}
