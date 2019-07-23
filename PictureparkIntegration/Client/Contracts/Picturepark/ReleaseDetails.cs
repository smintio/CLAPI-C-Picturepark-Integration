using Picturepark.SDK.V1.Contract;
using Picturepark.SDK.V1.Contract.Attributes;

namespace Client.Contracts.Picturepark
{
    [PictureparkSchema(SchemaType.Struct)]
    [PictureparkNameTranslation("x-default", "Smint.io Release Details")]
    [PictureparkNameTranslation("en", "Smint.io Release Details")]
    [PictureparkNameTranslation("de", "Smint.io Freigabe-Information")]
    public class ReleaseDetails
    {
        [PictureparkNameTranslation("x-default", "Model Release")]
        [PictureparkNameTranslation("en", "Model Release")]
        [PictureparkNameTranslation("de", "Modell-Freigabe")]
        public ReleaseState ModelReleaseState { get; set; }

        [PictureparkNameTranslation("x-default", "Property Release")]
        [PictureparkNameTranslation("en", "Property Release")]
        [PictureparkNameTranslation("de", "Eigentums-Freigabe")]
        public ReleaseState PropertyReleaseState { get; set; }

        [PictureparkNameTranslation("x-default", "Allowed Use")]
        [PictureparkNameTranslation("en", "Allowed Use")]
        [PictureparkNameTranslation("de", "Erlaubte Verwendung")]
        public TranslatedStringDictionary ProviderAllowedUseComment { get; set; }

        [PictureparkNameTranslation("x-default", "Release Comment")]
        [PictureparkNameTranslation("en", "Release Comment")]
        [PictureparkNameTranslation("de", "Freigabe-Kommentar")]
        public TranslatedStringDictionary ProviderReleaseComment { get; set; }

        [PictureparkNameTranslation("x-default", "Usage Constraints")]
        [PictureparkNameTranslation("en", "Usage Constraints")]
        [PictureparkNameTranslation("de", "Nutzungsbedingungen")]
        public TranslatedStringDictionary ProviderUsageConstraints { get; set; }
    }
}
