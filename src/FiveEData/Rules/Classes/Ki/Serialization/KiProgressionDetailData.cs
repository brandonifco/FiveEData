using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.Ki.Serialization;

internal sealed class KiProgressionDetailData
{
    [JsonRequired]
    public KiPointsGrantData[]? PointsByLevel { get; init; }

    [JsonRequired]
    public bool RecoversOnShortRest { get; init; }
}

internal sealed class KiPointsGrantData
{
    [JsonRequired]
    public int CharacterLevel { get; init; }

    [JsonRequired]
    public int Points { get; init; }
}
