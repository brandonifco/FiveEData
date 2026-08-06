using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;

namespace FiveEData.Rules.Classes.Metamagic.Serialization;

internal sealed class MetamagicOptionDefinitionData
{
    [JsonRequired]
    public string? Id { get; init; }

    [JsonRequired]
    public string? Name { get; init; }

    [JsonRequired]
    public int? FixedSorceryPointCost { get; init; }

    [JsonRequired]
    public bool CostEqualsSpellLevelWithCantripMinimum { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}
