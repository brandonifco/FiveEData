using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.DestroyUndead.Serialization;

internal sealed class DestroyUndeadProgressionDetailData
{
    [JsonRequired]
    public DestroyUndeadThresholdGrantData[]? ThresholdsByLevel { get; init; }
}

internal sealed class DestroyUndeadThresholdGrantData
{
    [JsonRequired]
    public int CharacterLevel { get; init; }

    [JsonRequired]
    public double MaxChallengeRating { get; init; }
}
