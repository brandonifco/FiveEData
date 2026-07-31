using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Equipment.AdventuringGear.Serialization;

internal sealed class AdventuringGearDefinitionData
{
    [JsonRequired]
    public string? Id { get; init; }

    [JsonRequired]
    public string? Name { get; init; }

    [JsonRequired]
    public MoneyData? Cost { get; init; }

    [JsonRequired]
    public ListedWeightData? ListedWeight { get; init; }

    [JsonRequired]
    public string[]? SpecialRuleIds { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}
