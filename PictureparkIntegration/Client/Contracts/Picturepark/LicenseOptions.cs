using Picturepark.SDK.V1.Contract;
using Picturepark.SDK.V1.Contract.Attributes;

namespace Client.Contracts.Picturepark
{
    [PictureparkSchema(SchemaType.Struct)]
    [PictureparkDisplayPattern(DisplayPatternType.Name, TemplateEngine.DotLiquid, "License options")]
    public class LicenseOptions
    {
        public TranslatedStringDictionary OptionName { get; set; }
        public TranslatedStringDictionary LicenseText { get; set; }
    }
}

