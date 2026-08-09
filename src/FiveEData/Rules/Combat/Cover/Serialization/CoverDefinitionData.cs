using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;

namespace FiveEData.Rules.Combat.Cover.Serialization;

internal sealed class CoverDefinitionData
{
    [JsonRequired]
    public string? Id { get; init; }

    [JsonRequired]
    public string? Name { get; init; }

    [JsonRequired]
    public int? ArmorClassBonus { get; init; }

    [JsonRequired]
    public int? DexteritySavingThrowBonus { get; init; }

    [JsonRequired]
    public bool PreventsBeingTargeted { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}
