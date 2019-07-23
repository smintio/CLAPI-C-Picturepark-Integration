using System;
using Picturepark.SDK.V1.Contract;
using Picturepark.SDK.V1.Contract.Attributes;

namespace Client.Contracts.Picturepark
{
    [PictureparkSchema(SchemaType.Layer)]
    [PictureparkNameTranslation("x-default", "Smint.io Asset")]
    [PictureparkNameTranslation("en", "Smint.io Asset")]
    [PictureparkNameTranslation("de", "Smint.io Asset")]
    [PictureparkDisplayPattern(DisplayPatternType.Name, TemplateEngine.DotLiquid, @"{{data.smintIo.name}} {% if data.smintIo.effectiveIsEditorialUse == true %} WARNING: This asset is for editorial use only!{% endif %}")]
    public class ContentLayer
    {
        [PictureparkRequired]
        [PictureparkNameTranslation("x-default", "Content Provider")]
        [PictureparkNameTranslation("en", "Content-Quelle")]
        [PictureparkNameTranslation("de", "Content-Quelle")]
        public ContentProvider ContentProvider { get; set; }

        [PictureparkRequired]
        [PictureparkNameTranslation("x-default", "Name")]
        [PictureparkNameTranslation("en", "Name")]
        [PictureparkNameTranslation("de", "Name")]
        public TranslatedStringDictionary Name { get; set; }

        [PictureparkNameTranslation("x-default", "Description")]
        [PictureparkNameTranslation("en", "Description")]
        [PictureparkNameTranslation("de", "Beschreibung")]
        public TranslatedStringDictionary Description { get; set; }

        [PictureparkNameTranslation("x-default", "Category")]
        [PictureparkNameTranslation("en", "Category")]
        [PictureparkNameTranslation("de", "Kategorie")]
        public ContentCategory Category { get; set; }

        [PictureparkNameTranslation("x-default", "Keywords")]
        [PictureparkNameTranslation("en", "Keywords")]
        [PictureparkNameTranslation("de", "Schlagwörter")]
        public TranslatedStringDictionary Keywords { get; set; }

        [PictureparkNameTranslation("x-default", "Copyright Notice")]
        [PictureparkNameTranslation("en", "Copyright Notice")]
        [PictureparkNameTranslation("de", "Copyright-Vermerk")]
        public TranslatedStringDictionary CopyrightNotices { get; set; }

        [PictureparkNameTranslation("x-default", "Project ID")]
        [PictureparkNameTranslation("en", "Project ID")]
        [PictureparkNameTranslation("de", "ID des Projekts")]
        public string ProjectUuid { get; set; }

        [PictureparkNameTranslation("x-default", "Project Name")]
        [PictureparkNameTranslation("en", "Project Name")]
        [PictureparkNameTranslation("de", "Projektname")]
        public TranslatedStringDictionary ProjectName { get; set; }

        [PictureparkNameTranslation("x-default", "Collection ID")]
        [PictureparkNameTranslation("en", "Collection ID")]
        [PictureparkNameTranslation("de", "ID der Sammlung")]
        public string CollectionUuid { get; set; }

        [PictureparkNameTranslation("x-default", "Collection Name")]
        [PictureparkNameTranslation("en", "Collection Name")]
        [PictureparkNameTranslation("de", "Name der Sammlung")]
        public TranslatedStringDictionary CollectionName { get; set; }

        [PictureparkNameTranslation("x-default", "Smint.io Link")]
        [PictureparkNameTranslation("en", "Smint.io Link")]
        [PictureparkNameTranslation("de", "Smint.io Link")]
        public string SmintIoUrl { get; set; }

        [PictureparkRequired]
        [PictureparkDateTime]
        [PictureparkNameTranslation("x-default", "Purchased At")]
        [PictureparkNameTranslation("en", "Purchased At")]
        [PictureparkNameTranslation("de", "Kaufdatum")]
        public DateTimeOffset PurchasedAt { get; set; }

        [PictureparkRequired]
        [PictureparkDateTime]
        [PictureparkNameTranslation("x-default", "Created At")]
        [PictureparkNameTranslation("en", "Created At")]
        [PictureparkNameTranslation("de", "Erzeugt")]
        public DateTimeOffset CreatedAt { get; set; }
    }
}
