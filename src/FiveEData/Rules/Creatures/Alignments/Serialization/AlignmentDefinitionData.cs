using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;

namespace FiveEData.Rules.Creatures.Alignments.Serialization;

internal sealed class AlignmentDefinitionData
{
    [JsonRequired]
    public string? Id { get; init; }

    [JsonRequired]
    public string? Name { get; init; }

    [JsonRequired]
    public AlignmentEthic Ethic { get; init; }

    [JsonRequired]
    public AlignmentMorality Morality { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}
