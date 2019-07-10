using Picturepark.SDK.V1.Contract;
using Picturepark.SDK.V1.Contract.Attributes;

namespace Client.Contracts.Picturepark
{
    [PictureparkSchema(SchemaType.Struct)]
    [PictureparkDisplayPattern(DisplayPatternType.Name, TemplateEngine.DotLiquid, "Download restrictions")]
    public class DownloadRestrictions
    {
        public int? EffectiveMaxUsers { get; set; }
        public int? EffectiveMaxUsages { get; set; }
        public int? EffectiveMaxDownloads { get; set; }
    }
}
