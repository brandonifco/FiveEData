using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;

namespace FiveEData.Rules.Characters.Encumbrance.Serialization;

internal sealed class EncumbranceRulesData
{
    [JsonRequired]
    public CarryingCapacitySizeMultiplierGrantData[]?
        SizeCarryingCapacityMultipliers
    { get; init; }

    [JsonRequired]
    public int EncumberedCarryingCapacityMultiplier { get; init; }

    [JsonRequired]
    public int EncumberedSpeedReductionFeet { get; init; }

    [JsonRequired]
    public int HeavilyEncumberedCarryingCapacityMultiplier { get; init; }

    [JsonRequired]
    public int HeavilyEncumberedSpeedReductionFeet { get; init; }

    [JsonRequired]
    public string[]? HeavilyEncumberedDisadvantageAbilityIds { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}
