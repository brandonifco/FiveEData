using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Classes.WardingFlare.Serialization;

internal sealed class WardingFlareDetailData
{
    [JsonRequired]
    public int TriggerRangeFeet { get; init; }

    [JsonRequired]
    public AbilityModifierUsesGrantData? UsesPerRest { get; init; }
}
