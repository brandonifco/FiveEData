using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Expenses.FoodAndLodging.Serialization;

internal sealed class LifestyleHospitalityCostDefinitionData
{
    [JsonRequired]
    public string? LifestyleId { get; init; }

    [JsonRequired]
    public MoneyData? InnStayCostPerDay { get; init; }

    [JsonRequired]
    public MoneyData? MealsCostPerDay { get; init; }

    [JsonRequired]
    public string[]? SpecialRuleIds { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}
