using Picturepark.SDK.V1.Contract;
using SmintIo.CLAPI.Consumer.Integration.Core.Target;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Client.Contracts
{
    public class PictureparkLicenseTerm : ISyncLicenseTerm
    {
        public DataDictionary Metadata { get; set; }

        public PictureparkLicenseTerm()
        {
            Metadata = new DataDictionary();
        }

        public void SetName(IDictionary<string, string> name)
        {
            Metadata.Add("name", name);
        }

        public void SetSequenceNumber(int sequenceNumber)
        {
            Metadata.Add("sequenceNumber", sequenceNumber);
        }

        public void SetExclusivities(IList<string> exclusivityKeys)
        {
            Metadata.Add("exclusivities", exclusivityKeys.Select(exclusivity => new { _refId = exclusivity }).ToArray());
        }

        public void SetAllowedUsages(IList<string> usageKeys)
        {
            Metadata.Add("allowedUsages", usageKeys.Select(usage => new { _refId = usage }).ToArray());
        }

        public void SetRestrictedUsages(IList<string> usageKeys)
        {
            Metadata.Add("restrictedUsages", usageKeys.Select(usage => new { _refId = usage }).ToArray());
        }

        public void SetAllowedSizes(IList<string> sizeKeys)
        {
            Metadata.Add("allowedSizes", sizeKeys.Select(usage => new { _refId = usage }).ToArray());
        }

        public void SetRestrictedSizes(IList<string> sizeKeys)
        {
            Metadata.Add("restrictedSizes", sizeKeys.Select(usage => new { _refId = usage }).ToArray());
        }

        public void SetAllowedPlacements(IList<string> placementKeys)
        {
            Metadata.Add("allowedPlacements", placementKeys.Select(usage => new { _refId = usage }).ToArray());
        }

        public void SetRestrictedPlacements(IList<string> placementKeys)
        {
            Metadata.Add("restrictedPlacements", placementKeys.Select(usage => new { _refId = usage }).ToArray());
        }

        public void SetAllowedDistributions(IList<string> distributionKeys)
        {
            Metadata.Add("allowedDistributions", distributionKeys.Select(usage => new { _refId = usage }).ToArray());
        }

        public void SetRestrictedDistributions(IList<string> distributionKeys)
        {
            Metadata.Add("restrictedDistributions", distributionKeys.Select(usage => new { _refId = usage }).ToArray());
        }

        public void SetAllowedGeographies(IList<string> geographyKeys)
        {
            Metadata.Add("allowedGeographies", geographyKeys.Select(usage => new { _refId = usage }).ToArray());
        }

        public void SetRestrictedGeographies(IList<string> geographyKeys)
        {
            Metadata.Add("restrictedGeographies", geographyKeys.Select(usage => new { _refId = usage }).ToArray());
        }

        public void SetAllowedIndustries(IList<string> industryKeys)
        {
            Metadata.Add("allowedIndustries", industryKeys.Select(usage => new { _refId = usage }).ToArray());
        }

        public void SetRestrictedIndustries(IList<string> industryKeys)
        {
            Metadata.Add("restrictedIndustries", industryKeys.Select(usage => new { _refId = usage }).ToArray());
        }

        public void SetAllowedLanguages(IList<string> languageKeys)
        {
            Metadata.Add("allowedLanguages", languageKeys.Select(usage => new { _refId = usage }).ToArray());
        }

        public void SetRestrictedLanguages(IList<string> languageKeys)
        {
            Metadata.Add("restrictedLanguages", languageKeys.Select(usage => new { _refId = usage }).ToArray());
        }

        public void SetUsageLimits(IList<string> usageLimitKeys)
        {
            Metadata.Add("usageLimits", usageLimitKeys.Select(usageLimit => new { _refId = usageLimit }).ToArray());
        }

        public void SetValidFrom(DateTimeOffset validFrom)
        {
            Metadata.Add("validFrom", validFrom);
        }

        public void SetValidUntil(DateTimeOffset validUntil)
        {
            Metadata.Add("validUntil", validUntil);
        }

        public void SetToBeUsedUntil(DateTimeOffset toBeUsedUntil)
        {
            Metadata.Add("toBeUsedUntil", toBeUsedUntil);
        }

        public void SetIsEditorialUse(bool isEditorialUse)
        {
            Metadata.Add("isEditorialUse", isEditorialUse);
        }
    }
}
