using Picturepark.SDK.V1.Contract;
using SmintIo.CLAPI.Consumer.Integration.Core.Target.Impl;
using System.Collections.Generic;

namespace Client.Contracts
{
    public class PictureparkLicenseOption : BaseSyncLicenseOption
    {
        public DataDictionary Metadata { get; set; }

        public PictureparkLicenseOption()
        {
            Metadata = new DataDictionary();
        }

        public override void SetName(IDictionary<string, string> name)
        {
            Metadata.Add("optionName", name);
        }

        public override void SetLicenseText(IDictionary<string, string> licenseText)
        {
            Metadata.Add("licenseText", licenseText);
        }
    }
}
