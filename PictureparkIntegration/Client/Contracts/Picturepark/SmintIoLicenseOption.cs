using Picturepark.SDK.V1.Contract;
using Picturepark.SDK.V1.Contract.Attributes;

namespace Client.Contracts.Picturepark
{
    [PictureparkSchema(SchemaType.Struct)]
    [PictureparkNameTranslation("x-default", "Smint.io License Option")]
    [PictureparkNameTranslation("en", "Smint.io License Option")]
    [PictureparkNameTranslation("de", "Smint.io Lizenzoption")]
    public class SmintIoLicenseOption
    {
        [PictureparkRequired]
        [PictureparkNameTranslation("x-default", "Name")]
        [PictureparkNameTranslation("en", "Name")]
        [PictureparkNameTranslation("de", "Name")]
        public TranslatedStringDictionary OptionName { get; set; }

        [PictureparkNameTranslation("x-default", "License Text")]
        [PictureparkNameTranslation("en", "License Text")]
        [PictureparkNameTranslation("de", "Lizenztext")]
        public TranslatedStringDictionary LicenseText { get; set; }
    }
}

