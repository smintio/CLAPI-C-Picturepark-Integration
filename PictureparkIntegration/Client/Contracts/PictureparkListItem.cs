using Picturepark.SDK.V1.Contract;

namespace Client.Contracts
{
    public sealed class PictureparkListItem
    {
        public string PictureparkListItemId { get; set; }

        public string SmintIoKey { get; set; }

        public DataDictionary Content { get; set; }
    }
}
