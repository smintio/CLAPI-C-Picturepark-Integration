using Picturepark.SDK.V1.Contract;
using Picturepark.SDK.V1.Contract.Attributes;

namespace Client.Contracts.Picturepark
{
    [PictureparkReference]
    [PictureparkSchema(SchemaType.List)]
    [PictureparkNameTranslation("x-default", "Smint.io Content Category")]
	[PictureparkNameTranslation("en", "Smint.io Content Category")]
    [PictureparkNameTranslation("de", "Smint.io Content-Kategorie")]
    [PictureparkDisplayPattern(DisplayPatternType.Name, TemplateEngine.DotLiquid, @"{{data.smintIoContentCategory.name | translate}}")]
    public class SmintIoContentCategory : ReferenceObject
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
