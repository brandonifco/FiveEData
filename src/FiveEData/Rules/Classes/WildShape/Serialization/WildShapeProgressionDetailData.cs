using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.WildShape.Serialization;

internal sealed class WildShapeProgressionDetailData
{
    [JsonRequired]
    public WildShapeFormLimitData[]? FormLimitsByLevel { get; init; }

    [JsonRequired]
    public int UsesPerRest { get; init; }

    [JsonRequired]
    public bool RecoversOnShortRest { get; init; }
}

internal sealed class WildShapeFormLimitData
{
    [JsonRequired]
    public int CharacterLevel { get; init; }

    [JsonRequired]
    public double MaxChallengeRating { get; init; }

    [JsonRequired]
    public bool AllowsFlyingSpeed { get; init; }

    [JsonRequired]
    public bool AllowsSwimmingSpeed { get; init; }
}
