using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.ThunderboltStrike.Serialization;

internal sealed class ThunderboltStrikeDetailData
{
    [JsonRequired]
    public int PushDistanceFeet { get; init; }

    [JsonRequired]
    public string? MaximumTargetSizeId { get; init; }
}
