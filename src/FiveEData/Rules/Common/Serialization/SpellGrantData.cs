using System.Text.Json.Serialization;

namespace FiveEData.Rules.Common.Serialization;

internal sealed class SpellGrantData
{
    [JsonRequired]
    public string? GrantedSpellId { get; init; }

    [JsonRequired]
    public int MinimumCharacterLevel { get; init; }

    [JsonRequired]
    public SpellGrantFrequency Frequency { get; init; }

    [JsonRequired]
    public int? CastAtSpellLevel { get; init; }
}
