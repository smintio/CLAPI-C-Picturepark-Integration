using System;
using System.Collections.Generic;
using Picturepark.SDK.V1.Contract.Attributes;
using SmintIo.CLAPI.Consumer.Client.Generated;

namespace Client.Contracts
{
    public sealed class SmintIoAsset
    {
        public string LPTUuid { get; set; }
        public string CartPTUuid { get; set; }

        public LicensePurchaseTransactionStateEnum? State { get; set; }

        public string Provider { get; set; }

        public IDictionary<string, string> Name { get; set; }
        public IDictionary<string, string> Description { get; set; }

        public IDictionary<string, string[]> Keywords { get; set; }

        public string Category { get; set; }

        public SmintIoReleaseDetails ReleaseDetails { get; set; }

        public IDictionary<string, string> CopyrightNotices { get; set; }

        public string ProjectUuid { get; set; }
        public IDictionary<string, string> ProjectName { get; set; }

        public string CollectionUuid { get; set; }
        public IDictionary<string, string> CollectionName { get; set; }

        public string LicenseeUuid { get; set; }
        public string LicenseeName { get; set; }

        public string LicenseType { get; set; }

        public IDictionary<string, string> LicenseText { get; set; }
        public IList<SmintIoLicenseOptions> LicenseOptions { get; set; }

        public IList<SmintIoUsageConstraints> UsageConstraints { get; set; }
        public SmintIoDownloadConstraints DownloadConstraints { get; set; }

        public bool? EffectiveIsEditorialUse { get; set; }

        public string DownloadUrl { get; set; }

        public string RecommendedFileName { get; set; }

        public string SmintIoUrl { get; set; }
        
        [PictureparkDateTime]
        public DateTimeOffset PurchasedAt { get; set; }
        [PictureparkDateTime]
        public DateTimeOffset? CreatedAt { get; set; }

        public DateTimeOffset? LptLastUpdatedAt { get; set; }
    }
}

