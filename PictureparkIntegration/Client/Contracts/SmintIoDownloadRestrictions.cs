namespace Client.Contracts
{
    public class SmintIoDownloadRestrictions
    {
        public int? EffectiveMaxUsers { get; set; }
        public int? EffectiveMaxUsages { get; set; }
        public int? EffectiveMaxDownloads { get; set; }
    }
}
