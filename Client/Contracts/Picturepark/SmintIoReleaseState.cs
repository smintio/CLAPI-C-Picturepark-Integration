using Picturepark.SDK.V1.Contract;
using Picturepark.SDK.V1.Contract.Attributes;

namespace Client.Contracts.Picturepark
{
    [PictureparkReference]
    [PictureparkSchema(SchemaType.List)]
    [PictureparkNameTranslation("x-default", "Smint.io Release State")]
    [PictureparkNameTranslation("en", "Smint.io Release State")]
    [PictureparkNameTranslation("de", "Smint.io Freigabestatus")]
    [PictureparkDisplayPattern(DisplayPatternType.Name, TemplateEngine.DotLiquid, @"{{data.smintIoReleaseState.name | translate}}")]
    public class SmintIoReleaseState : ReferenceObject
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
