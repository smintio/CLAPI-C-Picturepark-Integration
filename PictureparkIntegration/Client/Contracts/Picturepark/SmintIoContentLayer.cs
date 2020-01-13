using System;
using Picturepark.SDK.V1.Contract;
using Picturepark.SDK.V1.Contract.Attributes;

namespace Client.Contracts.Picturepark
{
    [PictureparkSchema(SchemaType.Layer)]
    [PictureparkNameTranslation("x-default", "Smint.io Asset")]
    [PictureparkNameTranslation("en", "Smint.io Asset")]
    [PictureparkNameTranslation("de", "Smint.io Asset")]
    [PictureparkDisplayPattern(DisplayPatternType.Name, TemplateEngine.DotLiquid, @"{% if data.smintIoContentLayer.isEditorialUse %} WARNING: This asset is for editorial use only! {% elsif data.smintIoContentLayer.hasRestrictiveLicenseTerms %} WARNING: This asset is subject to usage restrictions. Please adhere to the license terms! {% endif %}", "x-default")]
    [PictureparkDisplayPattern(DisplayPatternType.Name, TemplateEngine.DotLiquid, @"{% if data.smintIoContentLayer.isEditorialUse %} WARNING: This asset is for editorial use only! {% elsif data.smintIoContentLayer.hasRestrictiveLicenseTerms %} WARNING: WARNING: This asset is subject to usage restrictions. Please adhere to the license terms!! {% endif %}", "en")]
    [PictureparkDisplayPattern(DisplayPatternType.Name, TemplateEngine.DotLiquid, @"{% if data.smintIoContentLayer.isEditorialUse %} WARNUNG: Dieses Asset ist nur für den redaktionellen Gebrauch bestimmt! {% elsif data.smintIoContentLayer.hasRestrictiveLicenseTerms %} WARNUNG: Dieses Asset ist in der Nutzung eingeschränkt. Bitte beachten Sie die Lizenzbedingungen! {% endif %}", "de")]
    public class SmintIoContentLayer
    {
        [PictureparkNameTranslation("x-default", "Asset ID")]
        [PictureparkNameTranslation("en", "Asset ID")]
        [PictureparkNameTranslation("de", "Asset-ID")]
        public string ContentElementUuid { get; set; }

        [PictureparkRequired]
        [PictureparkNameTranslation("x-default", "Content Provider")]
        [PictureparkNameTranslation("en", "Content Provider")]
        [PictureparkNameTranslation("de", "Content-Quelle")]
        public SmintIoContentProvider ContentProvider { get; set; }

        [PictureparkRequired]
        [PictureparkNameTranslation("x-default", "Content Type")]
        [PictureparkNameTranslation("en", "Content Type")]
        [PictureparkNameTranslation("de", "Content-Typ")]
        public SmintIoContentType ContentType { get; set; }

        [PictureparkNameTranslation("x-default", "Binary Type")]
        [PictureparkNameTranslation("en", "Binary Type")]
        [PictureparkNameTranslation("de", "Datei-Typ")]
        public SmintIoBinaryType BinaryType { get; set; }

        [PictureparkRequired]
        [PictureparkNameTranslation("x-default", "Name")]
        [PictureparkNameTranslation("en", "Name")]
        [PictureparkNameTranslation("de", "Name")]
        public TranslatedStringDictionary Name { get; set; }

        [PictureparkNameTranslation("x-default", "Description")]
        [PictureparkNameTranslation("en", "Description")]
        [PictureparkNameTranslation("de", "Beschreibung")]
        [PictureparkString(MultiLine = true)]
        public TranslatedStringDictionary Description { get; set; }

        [PictureparkNameTranslation("x-default", "Content Category")]
        [PictureparkNameTranslation("en", "Content Category")]
        [PictureparkNameTranslation("de", "Content-Kategorie")]
        public SmintIoContentCategory Category { get; set; }

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
        public DateTimeOffset? PurchasedAt { get; set; }

        [PictureparkRequired]
        [PictureparkDateTime]
        [PictureparkNameTranslation("x-default", "Created At")]
        [PictureparkNameTranslation("en", "Created At")]
        [PictureparkNameTranslation("de", "Erzeugt")]
        public DateTimeOffset? CreatedAt { get; set; }

        [PictureparkDateTime]
        [PictureparkNameTranslation("x-default", "Last Updated At")]
        [PictureparkNameTranslation("en", "Last Updated At")]
        [PictureparkNameTranslation("de", "Aktualisiert")]
        public DateTimeOffset? LastUpdatedAt { get; set; }

        [PictureparkRequired]
        [PictureparkNameTranslation("x-default", "Transaction ID")]
        [PictureparkNameTranslation("en", "Transaction ID")]
        [PictureparkNameTranslation("de", "Transaktions-ID")]
        public string LicensePurchaseTransactionUuid { get; set; }

        [PictureparkRequired]
        [PictureparkNameTranslation("x-default", "Cart ID")]
        [PictureparkNameTranslation("en", "Cart ID")]
        [PictureparkNameTranslation("de", "ID des Warenkorbs")]
        public string CartPurchaseTransactionUuid { get; set; }

        // Not required for compound assets
        [PictureparkNameTranslation("x-default", "Binary ID")]
        [PictureparkNameTranslation("en", "Binary ID")]
        [PictureparkNameTranslation("de", "ID der Datei")]
        public string BinaryUuid { get; set; }

        // Not required for compound assets
        [PictureparkNameTranslation("x-default", "Binary Culture")]
        [PictureparkNameTranslation("en", "Binary Culture")]
        [PictureparkNameTranslation("de", "Kultur der Datei")]
        public string BinaryCulture { get; set; }

        // Not required for compound assets
        [PictureparkNameTranslation("x-default", "Binary Version")]
        [PictureparkNameTranslation("en", "Binary Version")]
        [PictureparkNameTranslation("de", "Version der Datei")]
        public int? BinaryVersion { get; set; }

        [PictureparkNameTranslation("x-default", "For Editorial Use Only")]
        [PictureparkNameTranslation("en", "For Editorial Use Only")]
        [PictureparkNameTranslation("de", "Nur für redaktionelle Nutzung")]
        public bool? IsEditorialUse { get; set; }

        [PictureparkNameTranslation("x-default", "Has Restrictive License Terms")]
        [PictureparkNameTranslation("en", "Has Restrictive License Terms")]
        [PictureparkNameTranslation("de", "Hat Lizenz-Einschränkungen")]
        public bool? HasRestrictiveLicenseTerms { get; set; }

        [PictureparkNameTranslation("x-default", "Has Been Cancelled")]
        [PictureparkNameTranslation("en", "Has Been Cancelled")]
        [PictureparkNameTranslation("de", "Wurde storniert")]
        public bool? HasBeenCancelled { get; set; }
    }
}
