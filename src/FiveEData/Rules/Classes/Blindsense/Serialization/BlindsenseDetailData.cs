using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.Blindsense.Serialization;

internal sealed class BlindsenseDetailData
{
    [JsonRequired]
    public int RangeFeet { get; init; }

    [JsonRequired]
    public bool RequiresHearing { get; init; }
}
