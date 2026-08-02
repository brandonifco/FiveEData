using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;

namespace FiveEData.Rules.Creatures.Languages.Serialization;

internal sealed class LanguageDefinitionData
{
    [JsonRequired]
    public string? Id { get; init; }

    [JsonRequired]
    public string? Name { get; init; }

    [JsonRequired]
    public LanguageCategory Category { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}
