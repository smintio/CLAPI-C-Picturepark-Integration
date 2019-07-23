using Picturepark.SDK.V1.Contract;
using Picturepark.SDK.V1.Contract.Attributes;

namespace Client.Contracts.Picturepark
{
    [PictureparkReference]
    [PictureparkSchema(SchemaType.List)]
    [PictureparkNameTranslation("x-default", "Smint.io License Size")]
    [PictureparkNameTranslation("en", "Smint.io License Size")]
    [PictureparkNameTranslation("de", "Smint.io Lizenz Größe")]
    public class LicenseSize : ReferenceObject
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
