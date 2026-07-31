using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Equipment.Ammunition.Serialization;

internal sealed class AmmunitionDefinitionData
{
    [JsonRequired]
    public string? Id { get; init; }

    [JsonRequired]
    public string? Name { get; init; }

    [JsonRequired]
    public int BundleQuantity { get; init; }

    [JsonRequired]
    public MoneyData? Cost { get; init; }

    [JsonRequired]
    public WeightData? Weight { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}
