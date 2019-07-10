using Picturepark.SDK.V1.Contract;
using Picturepark.SDK.V1.Contract.Attributes;

namespace Client.Contracts.Picturepark
{
    [PictureparkSchema(SchemaType.Struct)]
    [PictureparkDisplayPattern(DisplayPatternType.Name, TemplateEngine.DotLiquid, "Release details {% if data.releaseDetails.modelReleaseState %}{{data.releaseDetails.modelReleaseState}}{% endif %} ")]
    public class ReleaseDetails
    {
        public string ModelReleaseState { get; set; }
        public string PropertyReleaseState { get; set; }

        public TranslatedStringDictionary ProviderAllowedUseComment { get; set; }
        public TranslatedStringDictionary ProviderReleaseComment { get; set; }
        public TranslatedStringDictionary ProviderUsageRestrictions { get; set; }
    }
}
