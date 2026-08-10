using FiveEData.Rules.Common;

namespace FiveEData.Rules.Classes.WardingFlare;

public sealed record WardingFlareDetail
{
    public WardingFlareDetail(
        int triggerRangeFeet,
        AbilityModifierUsesGrant usesPerRest)
    {
        if (triggerRangeFeet <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(triggerRangeFeet),
                triggerRangeFeet,
                "Warding Flare trigger range must be greater than zero.");
        }

        TriggerRangeFeet = triggerRangeFeet;
        UsesPerRest = usesPerRest;
    }

    public int TriggerRangeFeet { get; }

    public AbilityModifierUsesGrant UsesPerRest { get; }
}
