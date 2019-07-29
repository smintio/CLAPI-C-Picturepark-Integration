using Picturepark.SDK.V1.Contract;
using System.Collections.Generic;

namespace Client.Contracts
{
    public sealed class PictureparkAsset
    {
        public string TransferId { get; set; }

        public string LPTUuid { get; set; }
        public string BinaryUuid { get; set; }
        public int BinaryVersion { get; set; }

        public string FindAgainFileUuid { get; set; }
        public string PictureparkContentId { get; set; }

        public DataDictionary Metadata { get; set; }

        public bool IsCompoundAsset { get; set; }

        public IList<PictureparkAsset> AssetParts { get; set; }

        public string RecommendedFileName { get; set; }

        public string DownloadUrl { get; set; }
        public string LocalFileName { get; set; }

        public IDictionary<string, string> Name { get; set; }
        public IDictionary<string, string> Usage { get; set; }
    }
}
