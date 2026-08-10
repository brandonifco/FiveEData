using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.DarkDelirium.Serialization;

internal sealed class DarkDeliriumDetailData
{
    [JsonRequired]
    public int RangeFeet { get; init; }

    [JsonRequired]
    public string? SavingThrowAbilityId { get; init; }

    [JsonRequired]
    public string[]? ChoosableConditionIds { get; init; }

    [JsonRequired]
    public int DurationMinutes { get; init; }

    [JsonRequired]
    public bool RequiresConcentration { get; init; }

    [JsonRequired]
    public bool RecoversOnShortRest { get; init; }
}
