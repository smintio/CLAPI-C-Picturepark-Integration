using Picturepark.SDK.V1.Contract;
using SmintIo.CLAPI.Consumer.Integration.Core.Contracts;

namespace Client.Contracts
{
    public sealed class PictureparkListItem
    {
        public string PictureparkListItemId { get; set; }

        public string SmintIoKey { get; set; }

        public DataDictionary Content { get; set; }

        public SmintIoMetadataElement SmintIoMetadataElement { get; set; }
    }
}
