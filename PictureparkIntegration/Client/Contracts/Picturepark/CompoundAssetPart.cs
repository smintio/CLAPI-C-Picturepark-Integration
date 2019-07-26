using Picturepark.SDK.V1.Contract;
using Picturepark.SDK.V1.Contract.Attributes;
using Picturepark.SDK.V1.Contract.SystemTypes;

namespace Client.Contracts.Picturepark
{
    [PictureparkSchema(SchemaType.Struct)]
    [PictureparkDisplayPattern(DisplayPatternType.Name, TemplateEngine.DotLiquid, "{{data.compoundAssetPart.usage | translate: language}}")]
    [PictureparkDisplayPattern(DisplayPatternType.List, TemplateEngine.DotLiquid, "{{data.compoundAssetPart.usage | translate: language}}")]
    [PictureparkDisplayPattern(DisplayPatternType.Thumbnail, TemplateEngine.DotLiquid, "{{data.compoundAssetPart.usage | translate: language}}")]
    [PictureparkDisplayPattern(DisplayPatternType.Detail, TemplateEngine.DotLiquid, "{{data.compoundAssetPart.usage | translate: language}}")]
    [PictureparkNameTranslation("x-default", "Smint.io Compound Asset Part")]
    [PictureparkNameTranslation("en", "Smint.io Compound Asset Part")]
    [PictureparkNameTranslation("de", "Smint.io Teil eines mehrteiligen Assets")]
    [PictureparkDescriptionTranslation("x-default", "A part of an Smint.io compound asset")]
    [PictureparkDescriptionTranslation("en", "A part of an Smint.io compound asset")]
    [PictureparkDescriptionTranslation("de", "Ein Teil eines mehrteiligen Smint.io Assets")]
    public class CompoundAssetPart : Relation
    {
        [PictureparkNameTranslation("x-default", "Usage")]
        [PictureparkNameTranslation("en", "Usage")]
        [PictureparkNameTranslation("de", "Verwendung")]        
        public TranslatedStringDictionary Usage { get; set; }
    }
}
