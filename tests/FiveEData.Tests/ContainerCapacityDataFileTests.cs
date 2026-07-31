using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment.AdventuringGear;
using FiveEData.Rules.Equipment.AdventuringGear.Serialization;

namespace FiveEData.Tests;

public sealed class ContainerCapacityDataFileTests
{
    [Fact]
    public void CanonicalData_MatchesFirstPrintingContainerCapacityTable()
    {
        IReadOnlyList<ContainerCapacityDefinition> definitions = LoadCanonical();

        Assert.Equal(13, definitions.Count);

        AssertCapacity(
            definitions,
            "dnd5e2014.adventuring-gear.backpack",
            solidAmount: 1m,
            solidUnit: ContainerVolumeUnit.CubicFoot,
            liquidAmount: null,
            liquidUnit: null,
            gearWeightPounds: 30m,
            allowsExteriorAttachment: true);
        AssertCapacity(
            definitions,
            "dnd5e2014.adventuring-gear.barrel",
            solidAmount: 4m,
            solidUnit: ContainerVolumeUnit.CubicFoot,
            liquidAmount: 40m,
            liquidUnit: ContainerVolumeUnit.Gallon,
            gearWeightPounds: null,
            allowsExteriorAttachment: false);
        AssertCapacity(
            definitions,
            "dnd5e2014.adventuring-gear.basket",
            solidAmount: 2m,
            solidUnit: ContainerVolumeUnit.CubicFoot,
            liquidAmount: null,
            liquidUnit: null,
            gearWeightPounds: 40m,
            allowsExteriorAttachment: false);
        AssertCapacity(
            definitions,
            "dnd5e2014.adventuring-gear.bottle-glass",
            solidAmount: null,
            solidUnit: null,
            liquidAmount: 1.5m,
            liquidUnit: ContainerVolumeUnit.Pint,
            gearWeightPounds: null,
            allowsExteriorAttachment: false);
        AssertCapacity(
            definitions,
            "dnd5e2014.adventuring-gear.bucket",
            solidAmount: 0.5m,
            solidUnit: ContainerVolumeUnit.CubicFoot,
            liquidAmount: 3m,
            liquidUnit: ContainerVolumeUnit.Gallon,
            gearWeightPounds: null,
            allowsExteriorAttachment: false);
        AssertCapacity(
            definitions,
            "dnd5e2014.adventuring-gear.chest",
            solidAmount: 12m,
            solidUnit: ContainerVolumeUnit.CubicFoot,
            liquidAmount: null,
            liquidUnit: null,
            gearWeightPounds: 300m,
            allowsExteriorAttachment: false);
        AssertCapacity(
            definitions,
            "dnd5e2014.adventuring-gear.flask-or-tankard",
            solidAmount: null,
            solidUnit: null,
            liquidAmount: 1m,
            liquidUnit: ContainerVolumeUnit.Pint,
            gearWeightPounds: null,
            allowsExteriorAttachment: false);
        AssertCapacity(
            definitions,
            "dnd5e2014.adventuring-gear.jug-or-pitcher",
            solidAmount: null,
            solidUnit: null,
            liquidAmount: 1m,
            liquidUnit: ContainerVolumeUnit.Gallon,
            gearWeightPounds: null,
            allowsExteriorAttachment: false);
        AssertCapacity(
            definitions,
            "dnd5e2014.adventuring-gear.pot-iron",
            solidAmount: null,
            solidUnit: null,
            liquidAmount: 1m,
            liquidUnit: ContainerVolumeUnit.Gallon,
            gearWeightPounds: null,
            allowsExteriorAttachment: false);
        AssertCapacity(
            definitions,
            "dnd5e2014.adventuring-gear.pouch",
            solidAmount: 0.2m,
            solidUnit: ContainerVolumeUnit.CubicFoot,
            liquidAmount: null,
            liquidUnit: null,
            gearWeightPounds: 6m,
            allowsExteriorAttachment: false);
        AssertCapacity(
            definitions,
            "dnd5e2014.adventuring-gear.sack",
            solidAmount: 1m,
            solidUnit: ContainerVolumeUnit.CubicFoot,
            liquidAmount: null,
            liquidUnit: null,
            gearWeightPounds: 30m,
            allowsExteriorAttachment: false);
        AssertCapacity(
            definitions,
            "dnd5e2014.adventuring-gear.vial",
            solidAmount: null,
            solidUnit: null,
            liquidAmount: 4m,
            liquidUnit: ContainerVolumeUnit.FluidOunce,
            gearWeightPounds: null,
            allowsExteriorAttachment: false);
        AssertCapacity(
            definitions,
            "dnd5e2014.adventuring-gear.waterskin",
            solidAmount: null,
            solidUnit: null,
            liquidAmount: 4m,
            liquidUnit: ContainerVolumeUnit.Pint,
            gearWeightPounds: null,
            allowsExteriorAttachment: false);
    }

    [Fact]
    public void CanonicalData_UsesPrintedPage153Provenance()
    {
        IReadOnlyList<ContainerCapacityDefinition> definitions = LoadCanonical();

        Assert.All(
            definitions,
            definition =>
            {
                SourceReference source = Assert.Single(definition.Sources);
                Assert.Equal(
                    "dnd5e2014.source.phb-first-printing",
                    source.DocumentId.Value);
                Assert.Equal(153, source.Page);
                Assert.Equal("Container Capacity", source.Section);
            });
    }

    private static IReadOnlyList<ContainerCapacityDefinition> LoadCanonical()
    {
        string root = FindRepositoryRoot();
        return ContainerCapacityDefinitionLoader.LoadFromFile(
            Path.Combine(
                root,
                "Data",
                "dnd5e2014",
                "container-capacities.json"));
    }

    private static void AssertCapacity(
        IReadOnlyList<ContainerCapacityDefinition> definitions,
        string gearId,
        decimal? solidAmount,
        ContainerVolumeUnit? solidUnit,
        decimal? liquidAmount,
        ContainerVolumeUnit? liquidUnit,
        decimal? gearWeightPounds,
        bool allowsExteriorAttachment)
    {
        ContainerCapacityDefinition definition = Assert.Single(
            definitions,
            item => item.AdventuringGearId.Value == gearId);

        Assert.Equal(solidAmount, definition.SolidVolume?.Amount);
        Assert.Equal(solidUnit, definition.SolidVolume?.Unit);
        Assert.Equal(liquidAmount, definition.LiquidVolume?.Amount);
        Assert.Equal(liquidUnit, definition.LiquidVolume?.Unit);
        Assert.Equal(gearWeightPounds, definition.GearWeightCapacity?.Pounds);
        Assert.Equal(
            allowsExteriorAttachment,
            definition.AllowsExteriorItemAttachment);
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory =
            new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "FiveEData.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not locate the FiveEData repository root.");
    }
}
