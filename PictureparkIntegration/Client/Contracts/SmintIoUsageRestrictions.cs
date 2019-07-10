using System;

namespace Client.Contracts
{
    public class SmintIoUsageRestrictions
    {
        public string EffectiveAllowedGeographies { get; set; }
        public string EffectiveRestrictedGeographies { get; set; }

        public DateTimeOffset? EffectiveValidFrom { get; set; }
        public DateTimeOffset? EffectiveValidUntil { get; set; }

        public DateTimeOffset? EffectiveToBeUsedUntil { get; set; }

        public bool EffectiveIsEditorialUse { get; set; }
    }
}
