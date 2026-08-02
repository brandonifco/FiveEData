using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;

namespace FiveEData.Rules.Creatures.Skills.Serialization;

internal sealed class SkillDefinitionData
{
    [JsonRequired]
    public string? Id { get; init; }

    [JsonRequired]
    public string? Name { get; init; }

    [JsonRequired]
    public string? NormallyAssociatedAbilityId { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}
