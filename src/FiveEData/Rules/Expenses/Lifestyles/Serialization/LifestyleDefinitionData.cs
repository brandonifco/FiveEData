using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Expenses.Lifestyles.Serialization;

internal sealed class LifestyleDefinitionData
{
    [JsonRequired]
    public string? Id { get; init; }

    [JsonRequired]
    public string? Name { get; init; }

    [JsonRequired]
    public ListedCostData? DailyCost { get; init; }

    [JsonRequired]
    public string[]? SpecialRuleIds { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}
