using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.SorceryPoints.Serialization;

internal sealed class SorceryPointsProgressionDetailData
{
    [JsonRequired]
    public SorceryPointsGrantData[]? PointsByLevel { get; init; }

    [JsonRequired]
    public bool RecoversOnShortRest { get; init; }
}

internal sealed class SorceryPointsGrantData
{
    [JsonRequired]
    public int CharacterLevel { get; init; }

    [JsonRequired]
    public int Points { get; init; }
}
