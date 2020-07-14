using Picturepark.SDK.V1.Contract;
using SmintIo.CLAPI.Consumer.Integration.Core.Target;

namespace Client.Contracts
{
    public class PictureparkDownloadConstraints : ISyncDownloadConstraints
    {
        public DataDictionary Metadata { get; set; }

        public PictureparkDownloadConstraints()
        {
            Metadata = new DataDictionary();
        }

        public void SetMaxUsers(int maxUsers)
        {
            Metadata.Add("maxUsers", maxUsers);
        }

        public void SetMaxDownloads(int maxDownloads)
        {
            Metadata.Add("maxDownloads", maxDownloads);
        }

        public void SetMaxReuses(int maxReuses)
        {
            Metadata.Add("maxReuses", maxReuses);
        }
    }
}
