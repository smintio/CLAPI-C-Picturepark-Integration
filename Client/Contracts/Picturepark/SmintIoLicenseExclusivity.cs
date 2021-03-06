﻿using Picturepark.SDK.V1.Contract;
using Picturepark.SDK.V1.Contract.Attributes;

namespace Client.Contracts.Picturepark
{
    [PictureparkReference]
    [PictureparkSchema(SchemaType.List)]
    [PictureparkNameTranslation("x-default", "Smint.io License Exclusivity")]
    [PictureparkNameTranslation("en", "Smint.io License Exclusivity")]
    [PictureparkNameTranslation("de", "Smint.io Lizenz Exklusivität")]
    [PictureparkDisplayPattern(DisplayPatternType.Name, TemplateEngine.DotLiquid, @"{{data.smintIoLicenseExclusivity.name | translate}}")]
    public class SmintIoLicenseExclusivity : ReferenceObject
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
