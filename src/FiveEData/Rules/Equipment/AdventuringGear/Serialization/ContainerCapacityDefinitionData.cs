using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;

namespace FiveEData.Rules.Equipment.AdventuringGear.Serialization;

internal sealed class ContainerCapacityDefinitionData
{
    [JsonRequired]
    public string? AdventuringGearId { get; init; }

    [JsonRequired]
    public ContainerVolumeData? SolidVolume { get; init; }

    [JsonRequired]
    public ContainerVolumeData? LiquidVolume { get; init; }

    [JsonRequired]
    public decimal? GearWeightCapacityPounds { get; init; }

    [JsonRequired]
    public bool AllowsExteriorItemAttachment { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}

internal sealed class ContainerVolumeData
{
    [JsonRequired]
    public decimal Amount { get; init; }

    [JsonRequired]
    public ContainerVolumeUnit Unit { get; init; }
}
