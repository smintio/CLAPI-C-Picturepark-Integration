using System.Collections.Generic;
using Picturepark.SDK.V1.Contract;
using Picturepark.SDK.V1.Contract.Attributes;

namespace Client.Contracts.Picturepark
{
    [PictureparkSchema(SchemaType.Layer)]
    [PictureparkNameTranslation("x-default", "Smint.io License Information")]
    [PictureparkNameTranslation("en", "Smint.io License Information")]
    [PictureparkNameTranslation("de", "Smint.io Lizenzinformation")]
    [PictureparkDisplayPattern(DisplayPatternType.Name, TemplateEngine.DotLiquid, @"{{data.smintIoLicense.name}} {% if data.smintIoLicense.effectiveIsEditorialUse == true %} WARNING: This asset is for editorial use only!{% endif %}")]
    public class LicenseLayer
    {
        [PictureparkRequired]
        [PictureparkNameTranslation("x-default", "Licensee Name")]
        [PictureparkNameTranslation("en", "Licensee Name")]
        [PictureparkNameTranslation("de", "Name des Lizenznehmers")]
        public string LicenseeName { get; set; }

        [PictureparkRequired]
        [PictureparkNameTranslation("x-default", "Licensee ID")]
        [PictureparkNameTranslation("en", "Licensee ID")]
        [PictureparkNameTranslation("de", "ID des Lizenznehmers")]
        public string LicenseeUuid { get; set; }

        [PictureparkRequired]
        [PictureparkNameTranslation("x-default", "License Type")]
        [PictureparkNameTranslation("en", "License Type")]
        [PictureparkNameTranslation("de", "Lizenztyp")]
        public LicenseType LicenseType { get; set; }

        [PictureparkNameTranslation("x-default", "License Text")]
        [PictureparkNameTranslation("en", "License Text")]
        [PictureparkNameTranslation("de", "Lizenztext")]
        public TranslatedStringDictionary LicenseText { get; set; }

        [PictureparkNameTranslation("x-default", "License Options")]
        [PictureparkNameTranslation("en", "License Options")]
        [PictureparkNameTranslation("de", "Lizenzoptionen")]
        public List<LicenseOption> LicenseOptions { get; set; }

        [PictureparkNameTranslation("x-default", "Usage Constraints")]
        [PictureparkNameTranslation("en", "Usage Constraints")]
        [PictureparkNameTranslation("de", "Nutzungsbedingungen")]
        public List<UsageConstraints> UsageConstraints { get; set; }

        [PictureparkNameTranslation("x-default", "Download Constraints")]
        [PictureparkNameTranslation("en", "Download Constraints")]
        [PictureparkNameTranslation("de", "Download-Bedingungen")]
        public DownloadConstraints DownloadConstraints { get; set; }

        [PictureparkNameTranslation("x-default", "Release Details")]
        [PictureparkNameTranslation("en", "Release Details")]
        [PictureparkNameTranslation("de", "Freigabeinformation")]
        public ReleaseDetails ReleaseDetails { get; set; }
        
        [PictureparkNameTranslation("x-default", "For Editorial Use Only")]
        [PictureparkNameTranslation("en", "For Editorial Use Only")]
        [PictureparkNameTranslation("de", "Nur für redaktionelle Nutzung")]
        public bool? EffectiveIsEditorialUse { get; set; }

        [PictureparkRequired]
        [PictureparkNameTranslation("x-default", "Has Been Cancelled")]
        [PictureparkNameTranslation("en", "Has Been Cancelled")]
        [PictureparkNameTranslation("de", "Wurde storniert")]
        public bool HasBeenCancelled { get; set; }
    }
}
