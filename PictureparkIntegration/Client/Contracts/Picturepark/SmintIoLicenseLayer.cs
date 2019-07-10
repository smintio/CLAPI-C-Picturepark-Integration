using System.Collections.Generic;
using Picturepark.SDK.V1.Contract;
using Picturepark.SDK.V1.Contract.Attributes;

namespace Client.Contracts.Picturepark
{
    [PictureparkSchema(SchemaType.Layer)]
    [PictureparkDisplayPattern(DisplayPatternType.Name, TemplateEngine.DotLiquid, @"{{data.smintIoLicense.name}} {% if data.smintIoLicense.effectiveIsEditorialUse == true %} WARNING: This asset is for editorial use only!{% endif %}")]
    public class SmintIoLicenseLayer
    {
        public string LicenseeName { get; set; }
        public string LicenseeUuid { get; set; }

        public string LicenseType { get; set; }
        public List<LicenseOptions> LicenseOptions { get; set; }

        public List<UsageRestrictions> UsageRestrictions { get; set; }
        public DownloadRestrictions DownloadRestrictions { get; set; }

        public ReleaseDetails ReleaseDetails { get; set; }

        public bool EffectiveIsEditorialUse { get; set; }
    }
}
