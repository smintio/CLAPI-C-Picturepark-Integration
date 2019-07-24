using Picturepark.SDK.V1.Contract;

namespace Client.Contracts
{
    public sealed class PictureparkAsset
    {
        public string TransferId { get; set; }

        public string Id { get; set; }

        public DataDictionary Metadata { get; set; }

        public string RecommendedFileName { get; set; }

        public string DownloadUrl { get; set; }
    }
}
