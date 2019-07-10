using System;
using Picturepark.SDK.V1.Contract;
using Picturepark.SDK.V1.Contract.Attributes;

namespace Client.Contracts.Picturepark
{
    [PictureparkSchema(SchemaType.Struct)]
    [PictureparkDisplayPattern(DisplayPatternType.Name, TemplateEngine.DotLiquid, "Usage restrictions")]
    public class UsageRestrictions
    {
        [PictureparkDateTime]
        public DateTimeOffset? EffectiveValidFrom { get; set; }
        [PictureparkDateTime]
        public DateTimeOffset? EffectiveValidUntil { get; set; }
        [PictureparkDateTime]
        public DateTimeOffset? EffectiveToBeUsedUntil { get; set; }

        public string EffectiveAllowedGeographies { get; set; }
        public string EffectiveRestrictedGeographies { get; set; }
    }
}
