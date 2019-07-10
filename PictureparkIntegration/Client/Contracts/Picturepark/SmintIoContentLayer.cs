using System;
using Picturepark.SDK.V1.Contract;
using Picturepark.SDK.V1.Contract.Attributes;

namespace Client.Contracts.Picturepark
{
    [PictureparkSchema(SchemaType.Layer)]
    [PictureparkDisplayPattern(DisplayPatternType.Name, TemplateEngine.DotLiquid, @"{{data.smintIo.name}} {% if data.smintIo.effectiveIsEditorialUse == true %} WARNING: This asset is for editorial use only!{% endif %}")]
    public class SmintIoContentLayer
    {
        public string Provider { get; set; }

        public TranslatedStringDictionary Name { get; set; }
        public TranslatedStringDictionary Description { get; set; }

        public TranslatedStringDictionary Categories { get; set; }
        public TranslatedStringDictionary Keywords { get; set; }

        public TranslatedStringDictionary CopyrightNotices { get; set; }

        public string ProjectUuid { get; set; }
        public TranslatedStringDictionary ProjectName { get; set; }

        public string CollectionUuid { get; set; }
        public TranslatedStringDictionary CollectionName { get; set; }

        public string SmintIoUrl { get; set; }

        [PictureparkDateTime]
        public DateTimeOffset PurchasedAt { get; set; }
        [PictureparkDateTime]
        public DateTimeOffset CreatedAt { get; set; }
    }
}
