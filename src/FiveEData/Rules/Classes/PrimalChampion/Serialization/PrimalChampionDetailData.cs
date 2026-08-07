using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.PrimalChampion.Serialization;

internal sealed class PrimalChampionDetailData
{
    [JsonRequired]
    public string[]? AbilityIds { get; init; }

    [JsonRequired]
    public int AbilityScoreIncrease { get; init; }

    [JsonRequired]
    public int MaximumAbilityScore { get; init; }
}
