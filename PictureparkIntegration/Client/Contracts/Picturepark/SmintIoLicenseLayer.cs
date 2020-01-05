using System.Collections.Generic;
using Picturepark.SDK.V1.Contract;
using Picturepark.SDK.V1.Contract.Attributes;

namespace Client.Contracts.Picturepark
{
    [PictureparkSchema(SchemaType.Layer)]
    [PictureparkNameTranslation("x-default", "Smint.io License Information")]
    [PictureparkNameTranslation("en", "Smint.io License Information")]
    [PictureparkNameTranslation("de", "Smint.io Lizenzinformation")]
    [PictureparkDisplayPattern(DisplayPatternType.Name, TemplateEngine.DotLiquid, @"{% if data.smintIoLicenseLayer.isEditorialUse %} WARNING: This asset is for editorial use only! {% elsif data.smintIoLicenseLayer.hasRestrictiveLicenseTerms %} WARNING: This asset is subject to usage restrictions. Please adhere to the license terms! {% endif %}", "x-default")]
    [PictureparkDisplayPattern(DisplayPatternType.Name, TemplateEngine.DotLiquid, @"{% if data.smintIoLicenseLayer.isEditorialUse %} WARNING: This asset is for editorial use only! {% elsif data.smintIoLicenseLayer.hasRestrictiveLicenseTerms %} WARNING: This asset is subject to usage restrictions. Please adhere to the license terms! {% endif %}", "en")]
    [PictureparkDisplayPattern(DisplayPatternType.Name, TemplateEngine.DotLiquid, @"{% if data.smintIoLicenseLayer.isEditorialUse %} WARNUNG: Dieses Asset ist nur für den redaktionellen Gebrauch bestimmt! {% elsif data.smintIoLicenseLayer.hasRestrictiveLicenseTerms %} WARNUNG: Dieses Asset ist in der Nutzung eingeschränkt. Bitte beachten Sie die Lizenzbedingungen! {% endif %}", "de")]
    public class SmintIoLicenseLayer
    {
        [PictureparkRequired]
        [PictureparkNameTranslation("x-default", "Licensee ID")]
        [PictureparkNameTranslation("en", "Licensee ID")]
        [PictureparkNameTranslation("de", "ID des Lizenznehmers")]
        public string LicenseeUuid { get; set; }

        [PictureparkRequired]
        [PictureparkNameTranslation("x-default", "Licensee Name")]
        [PictureparkNameTranslation("en", "Licensee Name")]
        [PictureparkNameTranslation("de", "Name des Lizenznehmers")]
        public string LicenseeName { get; set; }

        [PictureparkRequired]
        [PictureparkNameTranslation("x-default", "License Type")]
        [PictureparkNameTranslation("en", "License Type")]
        [PictureparkNameTranslation("de", "Lizenztyp")]
        public SmintIoLicenseType LicenseType { get; set; }

        [PictureparkNameTranslation("x-default", "License Text")]
        [PictureparkNameTranslation("en", "License Text")]
        [PictureparkNameTranslation("de", "Lizenztext")]
        [PictureparkString(MultiLine = true)]
        public TranslatedStringDictionary LicenseText { get; set; }

        [PictureparkNameTranslation("x-default", "Further Information")]
        [PictureparkNameTranslation("en", "Further Information")]
        [PictureparkNameTranslation("de", "Weiterführende Informationen")]
        public TranslatedStringDictionary LicenseUrls { get; set; }

        [PictureparkNameTranslation("x-default", "License Terms")]
        [PictureparkNameTranslation("en", "License Terms")]
        [PictureparkNameTranslation("de", "Lizenz-Bedingungen")]
        public List<SmintIoLicenseTerms> LicenseTerms { get; set; }

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
        public bool? IsEditorialUse { get; set; }

        [PictureparkNameTranslation("x-default", "Has Restrictive License Terms")]
        [PictureparkNameTranslation("en", "Has Restrictive License Terms")]
        [PictureparkNameTranslation("de", "Hat Lizenz-Einschränkungen")]
        public bool? HasRestrictiveLicenseTerms { get; set; }

        [PictureparkNameTranslation("x-default", "Has Been Cancelled")]
        [PictureparkNameTranslation("en", "Has Been Cancelled")]
        [PictureparkNameTranslation("de", "Wurde storniert")]
        public bool? HasBeenCancelled { get; set; }
    }
}
