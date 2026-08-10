using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.AwakenedMind.Serialization;

internal sealed class AwakenedMindDetailData
{
    [JsonRequired]
    public int TelepathyRangeFeet { get; init; }
}
