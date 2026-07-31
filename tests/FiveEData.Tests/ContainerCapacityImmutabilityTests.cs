using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment.AdventuringGear;

namespace FiveEData.Tests;

public sealed class ContainerCapacityImmutabilityTests
{
    [Fact]
    public void Definition_DefensivelySnapshotsSources()
    {
        var sources = new List<SourceReference>
        {
            new(
                new SourceDocumentId(
                    "dnd5e2014.source.phb-first-printing"),
                page: 153,
                section: "Container Capacity")
        };

        var definition = new ContainerCapacityDefinition(
            new AdventuringGearId(
                "dnd5e2014.adventuring-gear.backpack"),
            new ContainerVolume(1m, ContainerVolumeUnit.CubicFoot),
            liquidVolume: null,
            new Weight(30m),
            allowsExteriorItemAttachment: true,
            sources);

        sources.Clear();

        Assert.Single(definition.Sources);
    }
}
