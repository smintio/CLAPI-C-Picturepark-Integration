using Picturepark.SDK.V1.Contract;
using Picturepark.SDK.V1.Contract.Attributes;

namespace Client.Contracts.Picturepark
{
    [PictureparkSchema(SchemaType.Struct)]
    [PictureparkNameTranslation("x-default", "Smint.io Download Contraints")]
    [PictureparkNameTranslation("en", "Smint.io Download Constraints")]
    [PictureparkNameTranslation("de", "Smint.io Download-Bedingungen")]
    public class SmintIoDownloadConstraints
    {
        [PictureparkNameTranslation("x-default", "Download Limit")]
        [PictureparkNameTranslation("en", "Download Limit")]
        [PictureparkNameTranslation("de", "Download-Limit")]
        public int? MaxDownloads { get; set; }

        [PictureparkNameTranslation("x-default", "User Limit")]
        [PictureparkNameTranslation("en", "User Limit")]
        [PictureparkNameTranslation("de", "Benutzer-Limit")]
        public int? MaxUsers { get; set; }

        [PictureparkNameTranslation("x-default", "Reuse Limit")]
        [PictureparkNameTranslation("en", "Reuse Limit")]
        [PictureparkNameTranslation("de", "Wiederverwendungs-Limit")]
        public int? MaxReuses { get; set; }
    }
}
