using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment.AdventuringGear;

namespace FiveEData.Tests;

public sealed class ContainerCapacityCatalogTests
{
    [Fact]
    public void Constructor_DefensivelySnapshotsAndOrdersByGearId()
    {
        var source = new List<ContainerCapacityDefinition>
        {
            CreateDefinition("dnd5e2014.adventuring-gear.zeta"),
            CreateDefinition("dnd5e2014.adventuring-gear.alpha")
        };

        var catalog = new ContainerCapacityCatalog(source);
        source.Clear();

        Assert.Equal(2, catalog.Count);
        Assert.Equal(
            [
                "dnd5e2014.adventuring-gear.alpha",
                "dnd5e2014.adventuring-gear.zeta"
            ],
            catalog.All.Select(item => item.AdventuringGearId.Value).ToArray());
    }

    [Fact]
    public void Constructor_RejectsDuplicateGearIds()
    {
        ContainerCapacityDefinition first =
            CreateDefinition("dnd5e2014.adventuring-gear.same");
        ContainerCapacityDefinition second =
            CreateDefinition("dnd5e2014.adventuring-gear.same");

        Assert.Throws<ArgumentException>(
            () => new ContainerCapacityCatalog([first, second]));
    }

    [Fact]
    public void Constructor_RejectsDefaultVolumeState()
    {
        ContainerCapacityDefinition invalid = new(
            new AdventuringGearId("dnd5e2014.adventuring-gear.invalid"),
            solidVolume: default(ContainerVolume),
            liquidVolume: null,
            gearWeightCapacity: null,
            allowsExteriorItemAttachment: false,
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 153,
                    section: "Container Capacity")
            ]);

        Assert.Throws<InvalidOperationException>(
            () => new ContainerCapacityCatalog([invalid]));
    }

    [Fact]
    public void GetAndTryGet_HaveExplicitMissingSemantics()
    {
        var id = new AdventuringGearId("dnd5e2014.adventuring-gear.backpack");
        var catalog = new ContainerCapacityCatalog([CreateDefinition(id.Value)]);

        Assert.True(catalog.TryGet(id, out ContainerCapacityDefinition? found));
        Assert.NotNull(found);
        Assert.Same(found, catalog.Get(id));

        var missing = new AdventuringGearId("dnd5e2014.adventuring-gear.missing");
        Assert.False(catalog.TryGet(missing, out ContainerCapacityDefinition? absent));
        Assert.Null(absent);
        Assert.Throws<KeyNotFoundException>(() => catalog.Get(missing));
    }

    private static ContainerCapacityDefinition CreateDefinition(string id)
    {
        return new ContainerCapacityDefinition(
            new AdventuringGearId(id),
            new ContainerVolume(1m, ContainerVolumeUnit.CubicFoot),
            liquidVolume: null,
            new Weight(30m),
            allowsExteriorItemAttachment: false,
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 153,
                    section: "Container Capacity")
            ]);
    }
}
