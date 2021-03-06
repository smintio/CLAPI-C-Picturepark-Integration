﻿using Picturepark.SDK.V1.Contract;
using Picturepark.SDK.V1.Contract.Attributes;
using System.Collections.Generic;

namespace Client.Contracts.Picturepark
{
    [PictureparkSchema(SchemaType.Content)]
    [PictureparkDisplayPattern(DisplayPatternType.Detail, TemplateEngine.DotLiquid, @"Smint.io Compound Asset - {{data.smintIoCompoundAsset.name | translate: language}}", "x-default")]
    [PictureparkDisplayPattern(DisplayPatternType.Detail, TemplateEngine.DotLiquid, @"Smint.io Compound Asset - {{data.smintIoCompoundAsset.name | translate: language}}", "en")]
    [PictureparkDisplayPattern(DisplayPatternType.Detail, TemplateEngine.DotLiquid, @"Smint.io mehrteiliges Asset - {{data.smintIoCompoundAsset.name | translate: language}}", "de")]
    [PictureparkDisplayPattern(DisplayPatternType.List, TemplateEngine.DotLiquid, @"Smint.io Compound Asset - {{data.smintIoCompoundAsset.name | translate: language}}", "x-default")]
    [PictureparkDisplayPattern(DisplayPatternType.List, TemplateEngine.DotLiquid, @"Smint.io Compound Asset - {{data.smintIoCompoundAsset.name | translate: language}}", "en")]
    [PictureparkDisplayPattern(DisplayPatternType.List, TemplateEngine.DotLiquid, @"Smint.io mehrteiliges Asset - {{data.smintIoCompoundAsset.name | translate: language}}", "de")]
    [PictureparkDisplayPattern(DisplayPatternType.Name, TemplateEngine.DotLiquid, @"Smint.io Compound Asset - {{data.smintIoCompoundAsset.name | translate: language}}", "x-default")]
    [PictureparkDisplayPattern(DisplayPatternType.Name, TemplateEngine.DotLiquid, @"Smint.io Compound Asset - {{data.smintIoCompoundAsset.name | translate: language}}", "en")]
    [PictureparkDisplayPattern(DisplayPatternType.Name, TemplateEngine.DotLiquid, @"Smint.io mehrteiliges Asset - {{data.smintIoCompoundAsset.name | translate: language}}", "de")]
    [PictureparkDisplayPattern(DisplayPatternType.Thumbnail, TemplateEngine.DotLiquid, @"Smint.io Compound Asset - {{data.smintIoCompoundAsset.name | translate: language}}&nbsp;<img src=""https://www.smint.io/images/favicon.png"" width=""16""/>", "x-default")]
    [PictureparkDisplayPattern(DisplayPatternType.Thumbnail, TemplateEngine.DotLiquid, @"Smint.io Compound Asset - {{data.smintIoCompoundAsset.name | translate: language}}&nbsp;<img src=""https://www.smint.io/images/favicon.png"" width=""16""/>", "en")]
    [PictureparkDisplayPattern(DisplayPatternType.Thumbnail, TemplateEngine.DotLiquid, @"Smint.io mehrteiliges Asset - {{data.smintIoCompoundAsset.name | translate: language}}&nbsp;<img src=""https://www.smint.io/images/favicon.png"" width=""16""/>", "de")]
    [PictureparkNameTranslation("x-default", "Smint.io Compound Asset")]
    [PictureparkNameTranslation("en", "Smint.io Compound Asset")]
    [PictureparkNameTranslation("de", "Smint.io mehrteiliges Asset")]
    [PictureparkDescriptionTranslation("x-default", "Smint.io asset that consists of more than one part")]
    [PictureparkDescriptionTranslation("en", "Smint.io asset that consists of more than one part")]
    [PictureparkDescriptionTranslation("de", "Smint.io Asset, das aus mehr als einem Teil besteht")]
    public class SmintIoCompoundAsset
    {
        [PictureparkRequired]
        [PictureparkNameTranslation("x-default", "Name")]
        [PictureparkNameTranslation("en", "Name")]
        [PictureparkNameTranslation("de", "Name")]
        public TranslatedStringDictionary Name { get; set; }

        [PictureparkContentRelation("CompoundAssetPart", "")]
        [PictureparkNameTranslation("x-default", "Parts")]
        [PictureparkNameTranslation("en", "Parts")]
        [PictureparkNameTranslation("de", "Teile")]
        public List<SmintIoCompoundAssetPart> CompoundAssetParts { get; set; } = new List<SmintIoCompoundAssetPart>();
    }
}
