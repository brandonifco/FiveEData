using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment.Vehicles;

namespace FiveEData.Tests;

public sealed class VehicleCatalogTests
{
    [Fact]
    public void Catalog_OrdersByStableIdAndProvidesLookup()
    {
        var catalog = new VehicleCatalog(
            [
                Create("dnd5e2014.vehicle.z", "Z"),
                Create("dnd5e2014.vehicle.a", "A")
            ]);

        Assert.Equal(
            new[] { "dnd5e2014.vehicle.a", "dnd5e2014.vehicle.z" },
            catalog.All.Select(definition => definition.Id.Value).ToArray());
        Assert.Equal(
            "A",
            catalog.Get(new VehicleId("dnd5e2014.vehicle.a")).Name);

        Assert.True(
            catalog.TryGet(
                new VehicleId("dnd5e2014.vehicle.z"),
                out VehicleDefinition? found));
        Assert.NotNull(found);
    }

    [Fact]
    public void Catalog_DefensivelySnapshotsInput()
    {
        var source = new List<VehicleDefinition>
        {
            Create("dnd5e2014.vehicle.one", "One")
        };

        var catalog = new VehicleCatalog(source);
        source.Add(Create("dnd5e2014.vehicle.two", "Two"));

        Assert.Single(catalog.All);
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new VehicleCatalog(
                [
                    Create("dnd5e2014.vehicle.duplicate", "One"),
                    Create("dnd5e2014.vehicle.duplicate", "Two")
                ]));
    }

    [Fact]
    public void Catalog_RejectsDefaultIdAtTrustBoundary()
    {
        VehicleDefinition definition = new(
            default,
            "Invalid",
            VehicleKind.Land,
            new Money(100),
            listedWeight: new Weight(100),
            listedSpeed: null,
            specialRuleIds: [],
            sources:
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 157)
            ]);

        Assert.Throws<InvalidOperationException>(
            () => new VehicleCatalog([definition]));
    }

    private static VehicleDefinition Create(string id, string name)
    {
        return new VehicleDefinition(
            new VehicleId(id),
            name,
            VehicleKind.Land,
            new Money(100),
            listedWeight: new Weight(100),
            listedSpeed: null,
            specialRuleIds: [],
            sources:
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 157)
            ]);
    }
}
