using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;

namespace FiveEData.Rules.Creatures.Sizes.Serialization;

internal sealed class CreatureSizeDefinitionData
{
    [JsonRequired]
    public string? Id { get; init; }

    [JsonRequired]
    public string? Name { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}
