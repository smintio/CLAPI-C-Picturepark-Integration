using Picturepark.SDK.V1.Contract;
using Picturepark.SDK.V1.Contract.Attributes;

namespace Client.Contracts.Picturepark
{
    [PictureparkReference]
    [PictureparkSchema(SchemaType.List)]
    [PictureparkNameTranslation("x-default", "Smint.io License Language")]
    [PictureparkNameTranslation("en", "Smint.io License Language")]
    [PictureparkNameTranslation("de", "Smint.io Lizenz Sprache")]
    [PictureparkDisplayPattern(DisplayPatternType.Name, TemplateEngine.DotLiquid, @"{{data.smintIoLicenseLanguage.name | translate}}")]
    public class SmintIoLicenseLanguage : ReferenceObject
    {
        [PictureparkRequired]
        [PictureparkNameTranslation("x-default", "Key")]
        [PictureparkNameTranslation("en", "Key")]
        [PictureparkNameTranslation("de", "Key")]
        public string Key { get; set; }

        [PictureparkRequired]
        [PictureparkNameTranslation("x-default", "Name")]
        [PictureparkNameTranslation("en", "Name")]
        [PictureparkNameTranslation("de", "Name")]
        public TranslatedStringDictionary Name { get; set; }
    }
}
