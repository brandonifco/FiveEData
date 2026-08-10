using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.DraconicPresence.Serialization;

internal sealed class DraconicPresenceDetailData
{
    [JsonRequired]
    public int SorceryPointCost { get; init; }

    [JsonRequired]
    public int RangeFeet { get; init; }

    [JsonRequired]
    public string[]? ChoosableConditionIds { get; init; }

    [JsonRequired]
    public string? SavingThrowAbilityId { get; init; }

    [JsonRequired]
    public int DurationMinutes { get; init; }

    [JsonRequired]
    public bool RequiresConcentration { get; init; }
}
