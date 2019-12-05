using Picturepark.SDK.V1.Contract;
using SmintIo.CLAPI.Consumer.Integration.Core.Target.Impl;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Client.Contracts
{
    public class PictureparkLicenseTerm : BaseSyncLicenseTerm
    {
        public DataDictionary Metadata { get; set; }

        public PictureparkLicenseTerm()
        {
            Metadata = new DataDictionary();
        }

        public override void SetName(IDictionary<string, string> name)
        {
            Metadata.Add("name", name);
        }

        public override void SetSequenceNumber(int sequenceNumber)
        {
            Metadata.Add("sequenceNumber", sequenceNumber);
        }

        public override void SetExclusivities(IList<string> exclusivityKeys)
        {
            Metadata.Add("exclusivities", exclusivityKeys.Select(exclusivity => new { _refId = exclusivity }).ToArray());
        }

        public override void SetAllowedUsages(IList<string> usageKeys)
        {
            Metadata.Add("allowedUsages", usageKeys.Select(usage => new { _refId = usage }).ToArray());
        }

        public override void SetRestrictedUsages(IList<string> usageKeys)
        {
            Metadata.Add("restrictedUsages", usageKeys.Select(usage => new { _refId = usage }).ToArray());
        }

        public override void SetAllowedSizes(IList<string> sizeKeys)
        {
            Metadata.Add("allowedSizes", sizeKeys.Select(usage => new { _refId = usage }).ToArray());
        }

        public override void SetRestrictedSizes(IList<string> sizeKeys)
        {
            Metadata.Add("restrictedSizes", sizeKeys.Select(usage => new { _refId = usage }).ToArray());
        }

        public override void SetAllowedPlacements(IList<string> placementKeys)
        {
            Metadata.Add("allowedPlacements", placementKeys.Select(usage => new { _refId = usage }).ToArray());
        }

        public override void SetRestrictedPlacements(IList<string> placementKeys)
        {
            Metadata.Add("restrictedPlacements", placementKeys.Select(usage => new { _refId = usage }).ToArray());
        }

        public override void SetAllowedDistributions(IList<string> distributionKeys)
        {
            Metadata.Add("allowedDistributions", distributionKeys.Select(usage => new { _refId = usage }).ToArray());
        }

        public override void SetRestrictedDistributions(IList<string> distributionKeys)
        {
            Metadata.Add("restrictedDistributions", distributionKeys.Select(usage => new { _refId = usage }).ToArray());
        }

        public override void SetAllowedGeographies(IList<string> geographyKeys)
        {
            Metadata.Add("allowedGeographies", geographyKeys.Select(usage => new { _refId = usage }).ToArray());
        }

        public override void SetRestrictedGeographies(IList<string> geographyKeys)
        {
            Metadata.Add("restrictedGeographies", geographyKeys.Select(usage => new { _refId = usage }).ToArray());
        }

        public override void SetAllowedIndustries(IList<string> industryKeys)
        {
            Metadata.Add("allowedIndustries", industryKeys.Select(usage => new { _refId = usage }).ToArray());
        }

        public override void SetRestrictedIndustries(IList<string> industryKeys)
        {
            Metadata.Add("restrictedIndustries", industryKeys.Select(usage => new { _refId = usage }).ToArray());
        }

        public override void SetAllowedLanguages(IList<string> languageKeys)
        {
            Metadata.Add("allowedLanguages", languageKeys.Select(usage => new { _refId = usage }).ToArray());
        }

        public override void SetRestrictedLanguages(IList<string> languageKeys)
        {
            Metadata.Add("restrictedLanguages", languageKeys.Select(usage => new { _refId = usage }).ToArray());
        }

        public override void SetUsageLimits(IList<string> usageLimitKeys)
        {
            Metadata.Add("usageLimits", usageLimitKeys.Select(usageLimit => new { _refId = usageLimit }).ToArray());
        }

        public override void SetValidFrom(DateTimeOffset validFrom)
        {
            Metadata.Add("validFrom", validFrom);
        }

        public override void SetValidUntil(DateTimeOffset validUntil)
        {
            Metadata.Add("validUntil", validUntil);
        }

        public override void SetToBeUsedUntil(DateTimeOffset toBeUsedUntil)
        {
            Metadata.Add("toBeUsedUntil", toBeUsedUntil);
        }

        public override void SetIsEditorialUse(bool isEditorialUse)
        {
            Metadata.Add("isEditorialUse", isEditorialUse);
        }
    }
}
