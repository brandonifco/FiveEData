using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Classes.SongOfRest.Serialization;

internal sealed class SongOfRestProgressionDetailData
{
    [JsonRequired]
    public SongOfRestDieGrantData[]? DieByLevel { get; init; }
}

internal sealed class SongOfRestDieGrantData
{
    [JsonRequired]
    public int CharacterLevel { get; init; }

    [JsonRequired]
    public DiceExpressionData? Die { get; init; }
}
