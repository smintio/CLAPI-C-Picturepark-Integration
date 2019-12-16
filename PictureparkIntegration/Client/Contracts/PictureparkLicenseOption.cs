using Picturepark.SDK.V1.Contract;
using SmintIo.CLAPI.Consumer.Integration.Core.Target;
using System.Collections.Generic;

namespace Client.Contracts
{
    public class PictureparkLicenseOption : ISyncLicenseOption
    {
        public DataDictionary Metadata { get; set; }

        public PictureparkLicenseOption()
        {
            Metadata = new DataDictionary();
        }

        public void SetName(IDictionary<string, string> name)
        {
            Metadata.Add("optionName", name);
        }

        public void SetLicenseText(IDictionary<string, string> licenseText)
        {
            Metadata.Add("licenseText", licenseText);
        }
    }
}
