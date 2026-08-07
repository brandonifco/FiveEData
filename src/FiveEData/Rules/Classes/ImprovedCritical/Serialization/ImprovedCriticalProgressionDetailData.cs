using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.ImprovedCritical.Serialization;

internal sealed class ImprovedCriticalProgressionDetailData
{
    [JsonRequired]
    public CriticalHitThresholdGrantData[]? MinimumRollByLevel { get; init; }
}

internal sealed class CriticalHitThresholdGrantData
{
    [JsonRequired]
    public int CharacterLevel { get; init; }

    [JsonRequired]
    public int MinimumRoll { get; init; }
}
