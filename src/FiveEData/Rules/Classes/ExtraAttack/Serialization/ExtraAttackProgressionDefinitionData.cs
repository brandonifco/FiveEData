using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;

namespace FiveEData.Rules.Classes.ExtraAttack.Serialization;

internal sealed class ExtraAttackProgressionDefinitionData
{
    [JsonRequired]
    public string? Id { get; init; }

    [JsonRequired]
    public string? Name { get; init; }

    [JsonRequired]
    public ExtraAttackGrantData[]? Grants { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}

internal sealed class ExtraAttackGrantData
{
    [JsonRequired]
    public int CharacterLevel { get; init; }

    [JsonRequired]
    public int AttackCount { get; init; }
}
