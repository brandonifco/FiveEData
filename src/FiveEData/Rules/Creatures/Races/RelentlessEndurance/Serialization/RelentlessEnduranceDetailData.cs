using System.Text.Json.Serialization;

namespace FiveEData.Rules.Creatures.Races.RelentlessEndurance.Serialization;

internal sealed class RelentlessEnduranceDetailData
{
    [JsonRequired]
    public int HitPointsRetained { get; init; }

    [JsonRequired]
    public bool RecoversOnLongRest { get; init; }
}
