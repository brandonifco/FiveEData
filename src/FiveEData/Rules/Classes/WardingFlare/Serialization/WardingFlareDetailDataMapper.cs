using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Classes.WardingFlare.Serialization;

internal static class WardingFlareDetailDataMapper
{
    public static WardingFlareDetail Map(WardingFlareDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        AbilityModifierUsesGrantData usesPerRestData =
            data.UsesPerRest
            ?? throw new ArgumentException(
                "Warding Flare uses per rest is required.",
                nameof(data));

        return new WardingFlareDetail(
            data.TriggerRangeFeet,
            AbilityModifierUsesGrantDataMapper.Map(usesPerRestData));
    }
}
