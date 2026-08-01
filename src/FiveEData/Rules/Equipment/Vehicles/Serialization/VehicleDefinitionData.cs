using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Equipment.Vehicles.Serialization;

internal sealed class VehicleDefinitionData
{
    [JsonRequired]
    public string? Id { get; init; }

    [JsonRequired]
    public string? Name { get; init; }

    [JsonRequired]
    public VehicleKind Kind { get; init; }

    [JsonRequired]
    public MoneyData? Cost { get; init; }

    [JsonRequired]
    public WeightData? ListedWeight { get; init; }

    [JsonRequired]
    public decimal? ListedSpeedMilesPerHour { get; init; }

    [JsonRequired]
    public string[]? SpecialRuleIds { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}
