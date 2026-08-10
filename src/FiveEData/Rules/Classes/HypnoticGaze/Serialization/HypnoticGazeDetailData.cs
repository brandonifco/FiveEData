using System.Text.Json.Serialization;
using FiveEData.Rules.Common;

namespace FiveEData.Rules.Classes.HypnoticGaze.Serialization;

internal sealed class HypnoticGazeDetailData
{
    [JsonRequired]
    public int RangeFeet { get; init; }

    [JsonRequired]
    public string? SavingThrowAbilityId { get; init; }

    [JsonRequired]
    public string[]? ImposedConditionIds { get; init; }

    [JsonRequired]
    public bool SetsSpeedToZero { get; init; }

    [JsonRequired]
    public NextTurnDurationTrigger ConditionDurationTrigger { get; init; }
}
