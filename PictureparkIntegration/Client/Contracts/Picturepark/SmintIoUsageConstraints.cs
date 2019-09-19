using System;
using System.Collections.Generic;
using Picturepark.SDK.V1.Contract;
using Picturepark.SDK.V1.Contract.Attributes;

namespace Client.Contracts.Picturepark
{
    [PictureparkSchema(SchemaType.Struct)]
    [PictureparkNameTranslation("x-default", "Smint.io Usage Constraints")]
    [PictureparkNameTranslation("en", "Smint.io Usage Constraints")]
    [PictureparkNameTranslation("de", "Smint.io Nutzungsbedingungen")]
    public class SmintIoUsageConstraints
    {
        [PictureparkNameTranslation("x-default", "Sequence Number")]
        [PictureparkNameTranslation("en", "Sequence Number")]
        [PictureparkNameTranslation("de", "Sequenznummer")]
        public int? SequenceNumber { get; set; }

        [PictureparkNameTranslation("x-default", "Name")]
        [PictureparkNameTranslation("en", "Name")]
        [PictureparkNameTranslation("de", "Name")]
        public TranslatedStringDictionary Name { get; set; }

        [PictureparkNameTranslation("x-default", "Exclusivities")]
        [PictureparkNameTranslation("en", "Exclusivities")]
        [PictureparkNameTranslation("de", "Exklusivitäten")]
        public IList<SmintIoLicenseExclusivity> Exclusivities { get; set; }

        [PictureparkNameTranslation("x-default", "Allowed Usages")]
        [PictureparkNameTranslation("en", "Allowed Usages")]
        [PictureparkNameTranslation("de", "Erlaubte Nutzungen")]
        public IList<SmintIoLicenseUsage> AllowedUsages { get; set; }

        [PictureparkNameTranslation("x-default", "Restricted Usages")]
        [PictureparkNameTranslation("en", "Restricted Usages")]
        [PictureparkNameTranslation("de", "Nicht erlaubte Nutzungen")]
        public IList<SmintIoLicenseUsage> RestrictedUsages { get; set; }

        [PictureparkNameTranslation("x-default", "Allowed Sizes")]
        [PictureparkNameTranslation("en", "Allowed Sizes")]
        [PictureparkNameTranslation("de", "Erlaubte Größen")]
        public IList<SmintIoLicenseSize> AllowedSizes { get; set; }

        [PictureparkNameTranslation("x-default", "Restricted Sizes")]
        [PictureparkNameTranslation("en", "Restricted Sizes")]
        [PictureparkNameTranslation("de", "Nicht erlaubte Größen")]
        public IList<SmintIoLicenseSize> RestrictedSizes { get; set; }

        [PictureparkNameTranslation("x-default", "Allowed Placements")]
        [PictureparkNameTranslation("en", "Allowed Placements")]
        [PictureparkNameTranslation("de", "Erlaubte Platzierungen")]
        public IList<SmintIoLicensePlacement> AllowedPlacements { get; set; }

        [PictureparkNameTranslation("x-default", "Restricted Placements")]
        [PictureparkNameTranslation("en", "Restricted Placements")]
        [PictureparkNameTranslation("de", "Nicht erlaubte Platzierungen")]
        public IList<SmintIoLicensePlacement> RestrictedPlacements { get; set; }

        [PictureparkNameTranslation("x-default", "Allowed Distributions")]
        [PictureparkNameTranslation("en", "Allowed Distributions")]
        [PictureparkNameTranslation("de", "Erlaubte Verteilungen")]
        public IList<SmintIoLicenseDistribution> AllowedDistributions { get; set; }

        [PictureparkNameTranslation("x-default", "Restricted Distributions")]
        [PictureparkNameTranslation("en", "Restricted Distributions")]
        [PictureparkNameTranslation("de", "Nicht erlaubte Verteilungen")]
        public IList<SmintIoLicenseDistribution> RestrictedDistributions { get; set; }

        [PictureparkNameTranslation("x-default", "Allowed Geographies")]
        [PictureparkNameTranslation("en", "Allowed Geographies")]
        [PictureparkNameTranslation("de", "Erlaubte Regionen")]
        public IList<SmintIoLicenseGeography> AllowedGeographies { get; set; }

        [PictureparkNameTranslation("x-default", "Restricted Geographies")]
        [PictureparkNameTranslation("en", "Restricted Geographies")]
        [PictureparkNameTranslation("de", "Nicht erlaubte Regionen")]
        public IList<SmintIoLicenseGeography> RestrictedGeographies { get; set; }

        [PictureparkNameTranslation("x-default", "Allowed Industries")]
        [PictureparkNameTranslation("en", "Allowed Industries")]
        [PictureparkNameTranslation("de", "Erlaubte Industrien")]
        public IList<SmintIoLicenseVertical> AllowedVerticals { get; set; }

        [PictureparkNameTranslation("x-default", "Restricted Industries")]
        [PictureparkNameTranslation("en", "Restricted Industries")]
        [PictureparkNameTranslation("de", "Nicht erlaubte Industrien")]
        public IList<SmintIoLicenseVertical> RestrictedVerticals { get; set; }

        [PictureparkNameTranslation("x-default", "Allowed Languages")]
        [PictureparkNameTranslation("en", "Allowed Languages")]
        [PictureparkNameTranslation("de", "Erlaubte Sprachen")]
        public IList<SmintIoLicenseLanguage> AllowedLanguages { get; set; }

        [PictureparkNameTranslation("x-default", "Restricted Languages")]
        [PictureparkNameTranslation("en", "Restricted Languages")]
        [PictureparkNameTranslation("de", "Nicht erlaubte Sprachen")]
        public IList<SmintIoLicenseLanguage> RestrictedLanguages { get; set; }

        [PictureparkNameTranslation("x-default", "Editions Limit")]
        [PictureparkNameTranslation("en", "Editions Limit")]
        [PictureparkNameTranslation("de", "Maximale Auflage")]
        public int? MaxEditions { get; set; }

        [PictureparkRequired]
        [PictureparkDateTime]
        [PictureparkNameTranslation("x-default", "Valid From")]
        [PictureparkNameTranslation("en", "Valid From")]
        [PictureparkNameTranslation("de", "Gültig ab")]
        public DateTimeOffset? ValidFrom { get; set; }

        [PictureparkDateTime]
        [PictureparkNameTranslation("x-default", "Valid Until")]
        [PictureparkNameTranslation("en", "Valid Until")]
        [PictureparkNameTranslation("de", "Gültig bis")]
        public DateTimeOffset? ValidUntil { get; set; }
        [PictureparkDateTime]

        [PictureparkNameTranslation("x-default", "To Be Used Until")]
        [PictureparkNameTranslation("en", "To Be Used Until")]
        [PictureparkNameTranslation("de", "Lizenznutzung bis")]
        public DateTimeOffset? ToBeUsedUntil { get; set; }

        [PictureparkNameTranslation("x-default", "For Editorial Use Only")]
        [PictureparkNameTranslation("en", "For Editorial Use Only")]
        [PictureparkNameTranslation("de", "Nur für redaktionelle Nutzung")]
        public bool? IsEditorialUse { get; set; }
    }
}
