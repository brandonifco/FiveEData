using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.DivineSense.Serialization;

internal sealed class DivineSenseDetailData
{
    [JsonRequired]
    public int RangeFeet { get; init; }

    [JsonRequired]
    public bool RecoversOnLongRest { get; init; }
}
