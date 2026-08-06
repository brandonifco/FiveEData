using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Classes.BardicInspiration.Serialization;

internal sealed class BardicInspirationProgressionDetailData
{
    [JsonRequired]
    public BardicInspirationDieGrantData[]? DieByLevel { get; init; }

    [JsonRequired]
    public int RangeFeet { get; init; }

    [JsonRequired]
    public int DurationMinutes { get; init; }
}

internal sealed class BardicInspirationDieGrantData
{
    [JsonRequired]
    public int CharacterLevel { get; init; }

    [JsonRequired]
    public DiceExpressionData? Die { get; init; }
}
