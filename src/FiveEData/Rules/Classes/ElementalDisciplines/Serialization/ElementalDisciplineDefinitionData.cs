using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;

namespace FiveEData.Rules.Classes.ElementalDisciplines.Serialization;

internal sealed class ElementalDisciplineDefinitionData
{
    [JsonRequired]
    public string? Id { get; init; }

    [JsonRequired]
    public string? Name { get; init; }

    [JsonRequired]
    public int? KiPointCost { get; init; }

    [JsonRequired]
    public int? RequiredMinimumLevel { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}
