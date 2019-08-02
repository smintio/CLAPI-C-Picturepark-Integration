using System.Collections.Generic;
using Picturepark.SDK.V1.Contract;
using Picturepark.SDK.V1.Contract.Attributes;

namespace Client.Contracts.Picturepark
{
    [PictureparkSchema(SchemaType.Layer)]
    [PictureparkNameTranslation("x-default", "Smint.io License Information")]
    [PictureparkNameTranslation("en", "Smint.io License Information")]
    [PictureparkNameTranslation("de", "Smint.io Lizenzinformation")]
    [PictureparkDisplayPattern(DisplayPatternType.Name, TemplateEngine.DotLiquid, @"{% if data.smintIoLicenseLayer.effectiveIsEditorialUse %} WARNING: This asset is for editorial use only! {% elsif data.smintIoLicenseLayer.effectiveHasLicenseUsageConstraints %} WARNING: This asset is subject to license usage constraints! {% endif %}", "x-default")]
    [PictureparkDisplayPattern(DisplayPatternType.Name, TemplateEngine.DotLiquid, @"{% if data.smintIoLicenseLayer.effectiveIsEditorialUse %} WARNING: This asset is for editorial use only! {% elsif data.smintIoLicenseLayer.effectiveHasLicenseUsageConstraints %} WARNING: This asset is subject to license usage constraints! {% endif %}", "en")]
    [PictureparkDisplayPattern(DisplayPatternType.Name, TemplateEngine.DotLiquid, @"{% if data.smintIoLicenseLayer.effectiveIsEditorialUse %} WARNUNG: Dieses Asset ist nur für redaktionelle Nutzung gedacht! {% elsif data.smintIoLicenseLayer.effectiveHasLicenseUsageConstraints %} WARNUNG: Die Nutzung dieses Assets ist durch Nutzungsbedingungen eingeschränkt! {% endif %}", "de")]
    public class SmintIoLicenseLayer
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
        public SmintIoLicenseType LicenseType { get; set; }

        [PictureparkNameTranslation("x-default", "License Text")]
        [PictureparkNameTranslation("en", "License Text")]
        [PictureparkNameTranslation("de", "Lizenztext")]
        public TranslatedStringDictionary LicenseText { get; set; }

        [PictureparkNameTranslation("x-default", "License Options")]
        [PictureparkNameTranslation("en", "License Options")]
        [PictureparkNameTranslation("de", "Lizenzoptionen")]
        public List<SmintIoLicenseOption> LicenseOptions { get; set; }

        [PictureparkNameTranslation("x-default", "Usage Constraints")]
        [PictureparkNameTranslation("en", "Usage Constraints")]
        [PictureparkNameTranslation("de", "Nutzungsbedingungen")]
        public List<SmintIoUsageConstraints> UsageConstraints { get; set; }

        [PictureparkNameTranslation("x-default", "Download Constraints")]
        [PictureparkNameTranslation("en", "Download Constraints")]
        [PictureparkNameTranslation("de", "Download-Bedingungen")]
        public SmintIoDownloadConstraints DownloadConstraints { get; set; }

        [PictureparkNameTranslation("x-default", "Release Details")]
        [PictureparkNameTranslation("en", "Release Details")]
        [PictureparkNameTranslation("de", "Freigabeinformation")]
        public SmintIoReleaseDetails ReleaseDetails { get; set; }
        
        [PictureparkNameTranslation("x-default", "For Editorial Use Only")]
        [PictureparkNameTranslation("en", "For Editorial Use Only")]
        [PictureparkNameTranslation("de", "Nur für redaktionelle Nutzung")]
        public bool? EffectiveIsEditorialUse { get; set; }

        [PictureparkNameTranslation("x-default", "Has License Usage Constraints")]
        [PictureparkNameTranslation("en", "Has License Usage Constraints")]
        [PictureparkNameTranslation("de", "Hat Nutzungsbedingungen")]
        public bool? EffectiveHasLicenseUsageConstraints { get; set; }

        [PictureparkRequired]
        [PictureparkNameTranslation("x-default", "Has Been Cancelled")]
        [PictureparkNameTranslation("en", "Has Been Cancelled")]
        [PictureparkNameTranslation("de", "Wurde storniert")]
        public bool HasBeenCancelled { get; set; }
    }
}
