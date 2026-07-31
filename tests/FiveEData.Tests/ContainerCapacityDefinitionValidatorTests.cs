using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment.AdventuringGear;

namespace FiveEData.Tests;

public sealed class ContainerCapacityDefinitionValidatorTests
{
    [Fact]
    public void ValidDefinition_IsAccepted()
    {
        ContainerCapacityDefinition definition = CreateValidDefinition();

        Assert.Empty(ContainerCapacityDefinitionValidator.Validate(definition));
    }

    [Fact]
    public void DefaultGearId_IsRejected()
    {
        ContainerCapacityDefinition definition = new(
            default,
            new ContainerVolume(1m, ContainerVolumeUnit.CubicFoot),
            liquidVolume: null,
            new Weight(30m),
            allowsExteriorItemAttachment: false,
            Sources);

        Assert.Contains(
            ContainerCapacityDefinitionValidator.Validate(definition),
            error => error.Contains("ID", StringComparison.Ordinal));
    }

    [Fact]
    public void DefinitionWithoutCapacityMeasure_IsRejected()
    {
        ContainerCapacityDefinition definition = new(
            new AdventuringGearId("dnd5e2014.adventuring-gear.test"),
            solidVolume: null,
            liquidVolume: null,
            gearWeightCapacity: null,
            allowsExteriorItemAttachment: false,
            Sources);

        Assert.Contains(
            ContainerCapacityDefinitionValidator.Validate(definition),
            error => error.Contains("at least one", StringComparison.Ordinal));
    }

    [Fact]
    public void DefaultVolume_IsRejectedAtAggregateBoundary()
    {
        ContainerCapacityDefinition definition = new(
            new AdventuringGearId("dnd5e2014.adventuring-gear.test"),
            solidVolume: default(ContainerVolume),
            liquidVolume: null,
            gearWeightCapacity: null,
            allowsExteriorItemAttachment: false,
            Sources);

        Assert.NotEmpty(ContainerCapacityDefinitionValidator.Validate(definition));
    }

    [Fact]
    public void SolidVolumeUsingLiquidUnit_IsRejected()
    {
        ContainerCapacityDefinition definition = new(
            new AdventuringGearId("dnd5e2014.adventuring-gear.test"),
            new ContainerVolume(1m, ContainerVolumeUnit.Gallon),
            liquidVolume: null,
            gearWeightCapacity: null,
            allowsExteriorItemAttachment: false,
            Sources);

        Assert.Contains(
            ContainerCapacityDefinitionValidator.Validate(definition),
            error => error.Contains("cubic feet", StringComparison.Ordinal));
    }

    [Fact]
    public void LiquidVolumeUsingSolidUnit_IsRejected()
    {
        ContainerCapacityDefinition definition = new(
            new AdventuringGearId("dnd5e2014.adventuring-gear.test"),
            solidVolume: null,
            new ContainerVolume(1m, ContainerVolumeUnit.CubicFoot),
            gearWeightCapacity: null,
            allowsExteriorItemAttachment: false,
            Sources);

        Assert.Contains(
            ContainerCapacityDefinitionValidator.Validate(definition),
            error => error.Contains("liquid-volume", StringComparison.Ordinal));
    }

    private static ContainerCapacityDefinition CreateValidDefinition()
    {
        return new ContainerCapacityDefinition(
            new AdventuringGearId("dnd5e2014.adventuring-gear.test"),
            new ContainerVolume(1m, ContainerVolumeUnit.CubicFoot),
            liquidVolume: null,
            new Weight(30m),
            allowsExteriorItemAttachment: false,
            Sources);
    }

    private static SourceReference[] Sources =>
    [
        new SourceReference(
            new SourceDocumentId("dnd5e2014.source.phb-first-printing"),
            page: 153,
            section: "Container Capacity")
    ];
}
