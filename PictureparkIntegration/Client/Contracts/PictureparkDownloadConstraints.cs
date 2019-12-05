using Picturepark.SDK.V1.Contract;
using SmintIo.CLAPI.Consumer.Integration.Core.Target.Impl;

namespace Client.Contracts
{
    public class PictureparkDownloadConstraints : BaseSyncDownloadConstraints
    {
        public DataDictionary Metadata { get; set; }

        public PictureparkDownloadConstraints()
        {
            Metadata = new DataDictionary();
        }

        public override void SetMaxUsers(int maxUsers)
        {
            Metadata.Add("maxUsers", maxUsers);
        }

        public override void SetMaxDownloads(int maxDownloads)
        {
            Metadata.Add("maxDownloads", maxDownloads);
        }

        public override void SetMaxReuses(int maxReuses)
        {
            Metadata.Add("maxReuses", maxReuses);
        }
    }
}
