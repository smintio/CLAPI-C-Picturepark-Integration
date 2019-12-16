using Picturepark.SDK.V1.Contract;
using SmintIo.CLAPI.Consumer.Integration.Core.Target;
using System.Collections.Generic;

namespace Client.Contracts
{
    public class PictureparkReleaseDetails : ISyncReleaseDetails
    {
        public DataDictionary Metadata { get; set; }

        public PictureparkReleaseDetails()
        {
            Metadata = new DataDictionary();
        }

        public void SetModelReleaseState(string modelReleaseStateKey)
        {
            Metadata.Add("modelReleaseState", new { _refId = modelReleaseStateKey });
        }

        public void SetPropertyReleaseState(string propertyReleaseStateKey)
        {
            Metadata.Add("propertyReleaseState", new { _refId = propertyReleaseStateKey });
        }

        public void SetProviderAllowedUseComment(IDictionary<string, string> providerAllowedUseComment)
        {
            Metadata.Add("providerAllowedUseComment", providerAllowedUseComment);
        }

        public void SetProviderReleaseComment(IDictionary<string, string> providerReleaseComment)
        {
            Metadata.Add("providerReleaseComment", providerReleaseComment);
        }

        public void SetProviderUsageConstraints(IDictionary<string, string> providerUsageConstraints)
        {
            Metadata.Add("providerUsageConstraints", providerUsageConstraints);
        }
    }
}
