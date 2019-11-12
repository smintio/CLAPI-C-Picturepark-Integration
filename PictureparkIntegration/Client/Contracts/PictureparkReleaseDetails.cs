using Picturepark.SDK.V1.Contract;
using SmintIo.CLAPI.Consumer.Integration.Core.Target.Impl;
using System.Collections.Generic;

namespace Client.Contracts
{
    public class PictureparkReleaseDetails : SyncReleaseDetailsImpl
    {
        public DataDictionary Metadata { get; set; }

        public PictureparkReleaseDetails()
        {
            Metadata = new DataDictionary();
        }

        public override void SetModelReleaseState(string modelReleaseStateKey)
        {
            Metadata.Add("modelReleaseState", new { _refId = modelReleaseStateKey });
        }

        public override void SetPropertyReleaseState(string propertyReleaseStateKey)
        {
            Metadata.Add("propertyReleaseState", new { _refId = propertyReleaseStateKey });
        }

        public override void SetProviderAllowedUseComment(IDictionary<string, string> providerAllowedUseComment)
        {
            Metadata.Add("providerAllowedUseComment", providerAllowedUseComment);
        }

        public override void SetProviderReleaseComment(IDictionary<string, string> providerReleaseComment)
        {
            Metadata.Add("providerReleaseComment", providerReleaseComment);
        }

        public override void SetProviderUsageConstraints(IDictionary<string, string> providerUsageConstraints)
        {
            Metadata.Add("providerUsageConstraints", providerUsageConstraints);
        }
    }
}
