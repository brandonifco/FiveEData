using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.MistyEscape.Serialization;

internal sealed class MistyEscapeDetailData
{
    [JsonRequired]
    public int TeleportRangeFeet { get; init; }

    [JsonRequired]
    public bool GrantsInvisibility { get; init; }

    [JsonRequired]
    public bool RecoversOnShortRest { get; init; }
}
