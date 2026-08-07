using System.Text.Json.Serialization;

namespace FiveEData.Rules.Creatures.Races.Lucky.Serialization;

internal sealed class LuckyDetailData
{
    [JsonRequired]
    public int RerollOnNaturalRoll { get; init; }

    [JsonRequired]
    public bool MustUseNewRoll { get; init; }
}
