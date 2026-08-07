using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.EmptyBody.Serialization;

internal sealed class EmptyBodyDetailData
{
    [JsonRequired]
    public int InvisibilityKiCost { get; init; }

    [JsonRequired]
    public int InvisibilityDurationMinutes { get; init; }

    [JsonRequired]
    public int AstralProjectionKiCost { get; init; }
}
