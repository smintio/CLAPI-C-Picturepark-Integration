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
        [PictureparkNameTranslation("x-default", "Is Exclusive")]
        [PictureparkNameTranslation("en", "Is Exclusive")]
        [PictureparkNameTranslation("de", "Ist exklusiv")]
        public IList<SmintIoLicenseUsage> EffectiveIsExclusive { get; set; }

        [PictureparkNameTranslation("x-default", "Allowed Usages")]
        [PictureparkNameTranslation("en", "Allowed Usages")]
        [PictureparkNameTranslation("de", "Erlaubte Nutzungen")]
        public IList<SmintIoLicenseUsage> EffectiveAllowedUsages { get; set; }

        [PictureparkNameTranslation("x-default", "Restricted Usages")]
        [PictureparkNameTranslation("en", "Restricted Usages")]
        [PictureparkNameTranslation("de", "Nicht erlaubte Nutzungen")]
        public IList<SmintIoLicenseUsage> EffectiveRestrictedUsages { get; set; }

        [PictureparkNameTranslation("x-default", "Allowed Sizes")]
        [PictureparkNameTranslation("en", "Allowed Sizes")]
        [PictureparkNameTranslation("de", "Erlaubte Größen")]
        public IList<SmintIoLicenseSize> EffectiveAllowedSizes { get; set; }

        [PictureparkNameTranslation("x-default", "Restricted Sizes")]
        [PictureparkNameTranslation("en", "Restricted Sizes")]
        [PictureparkNameTranslation("de", "Nicht erlaubte Größen")]
        public IList<SmintIoLicenseSize> EffectiveRestrictedSizes { get; set; }

        [PictureparkNameTranslation("x-default", "Allowed Placements")]
        [PictureparkNameTranslation("en", "Allowed Placements")]
        [PictureparkNameTranslation("de", "Erlaubte Platzierungen")]
        public IList<SmintIoLicensePlacement> EffectiveAllowedPlacements { get; set; }

        [PictureparkNameTranslation("x-default", "Restricted Placements")]
        [PictureparkNameTranslation("en", "Restricted Placements")]
        [PictureparkNameTranslation("de", "Nicht erlaubte Platzierungen")]
        public IList<SmintIoLicensePlacement> EffectiveRestrictedPlacements { get; set; }

        [PictureparkNameTranslation("x-default", "Allowed Distributions")]
        [PictureparkNameTranslation("en", "Allowed Distributions")]
        [PictureparkNameTranslation("de", "Erlaubte Verteilungen")]
        public IList<SmintIoLicenseDistribution> EffectiveAllowedDistributions { get; set; }

        [PictureparkNameTranslation("x-default", "Restricted Distributions")]
        [PictureparkNameTranslation("en", "Restricted Distributions")]
        [PictureparkNameTranslation("de", "Nicht erlaubte Verteilungen")]
        public IList<SmintIoLicenseDistribution> EffectiveRestrictedDistributions { get; set; }

        [PictureparkNameTranslation("x-default", "Allowed Geographies")]
        [PictureparkNameTranslation("en", "Allowed Geographies")]
        [PictureparkNameTranslation("de", "Erlaubte Regionen")]
        public IList<SmintIoLicenseGeography> EffectiveAllowedGeographies { get; set; }

        [PictureparkNameTranslation("x-default", "Restricted Geographies")]
        [PictureparkNameTranslation("en", "Restricted Geographies")]
        [PictureparkNameTranslation("de", "Nicht erlaubte Regionen")]
        public IList<SmintIoLicenseGeography> EffectiveRestrictedGeographies { get; set; }

        [PictureparkNameTranslation("x-default", "Allowed Verticals")]
        [PictureparkNameTranslation("en", "Allowed Verticals")]
        [PictureparkNameTranslation("de", "Erlaubte Industrien")]
        public IList<SmintIoLicenseVertical> EffectiveAllowedVerticals { get; set; }

        [PictureparkNameTranslation("x-default", "Restricted Verticals")]
        [PictureparkNameTranslation("en", "Restricted Verticals")]
        [PictureparkNameTranslation("de", "Nicht erlaubte Industrien")]
        public IList<SmintIoLicenseVertical> EffectiveRestrictedVerticals { get; set; }

        [PictureparkRequired]
        [PictureparkDateTime]
        [PictureparkNameTranslation("x-default", "Valid From")]
        [PictureparkNameTranslation("en", "Valid From")]
        [PictureparkNameTranslation("de", "Gültig ab")]
        public DateTimeOffset? EffectiveValidFrom { get; set; }

        [PictureparkDateTime]
        [PictureparkNameTranslation("x-default", "Valid Until")]
        [PictureparkNameTranslation("en", "Valid Until")]
        [PictureparkNameTranslation("de", "Gültig bis")]
        public DateTimeOffset? EffectiveValidUntil { get; set; }
        [PictureparkDateTime]

        [PictureparkNameTranslation("x-default", "To Be Used Until")]
        [PictureparkNameTranslation("en", "To Be Used Until")]
        [PictureparkNameTranslation("de", "Lizenznutzung bis")]
        public DateTimeOffset? EffectiveToBeUsedUntil { get; set; }

        [PictureparkNameTranslation("x-default", "For Editorial Use Only")]
        [PictureparkNameTranslation("en", "For Editorial Use Only")]
        [PictureparkNameTranslation("de", "Nur für redaktionelle Nutzung")]
        public bool? EffectiveIsEditorialUse { get; set; }
    }
}
