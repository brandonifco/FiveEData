using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.Auras.Serialization;

internal sealed class AuraRangeData
{
    [JsonRequired]
    public int BaseRangeFeet { get; init; }

    [JsonRequired]
    public int ExpandedRangeFeet { get; init; }

    [JsonRequired]
    public int ExpandedAtLevel { get; init; }
}
