using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment.Mounts;

namespace FiveEData.Tests;

public sealed class MountCatalogTests
{
    [Fact]
    public void Catalog_OrdersByStableIdAndProvidesLookup()
    {
        var catalog = new MountCatalog(
            [
                Create("dnd5e2014.mount.z", "Z"),
                Create("dnd5e2014.mount.a", "A")
            ]);

        Assert.Equal(
            new[] { "dnd5e2014.mount.a", "dnd5e2014.mount.z" },
            catalog.All.Select(definition => definition.Id.Value).ToArray());
        Assert.Equal(
            "A",
            catalog.Get(new MountId("dnd5e2014.mount.a")).Name);

        Assert.True(
            catalog.TryGet(
                new MountId("dnd5e2014.mount.z"),
                out MountDefinition? found));
        Assert.NotNull(found);
    }

    [Fact]
    public void Catalog_DefensivelySnapshotsInput()
    {
        var source = new List<MountDefinition>
        {
            Create("dnd5e2014.mount.one", "One")
        };

        var catalog = new MountCatalog(source);
        source.Add(Create("dnd5e2014.mount.two", "Two"));

        Assert.Single(catalog.All);
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new MountCatalog(
                [
                    Create("dnd5e2014.mount.duplicate", "One"),
                    Create("dnd5e2014.mount.duplicate", "Two")
                ]));
    }

    [Fact]
    public void Catalog_RejectsDefaultIdAtTrustBoundary()
    {
        MountDefinition definition = new(
            default,
            "Invalid",
            new Money(100),
            new Distance(40),
            new Weight(100),
            specialRuleIds: [],
            sources:
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 155)
            ]);

        Assert.Throws<InvalidOperationException>(
            () => new MountCatalog([definition]));
    }

    private static MountDefinition Create(string id, string name)
    {
        return new MountDefinition(
            new MountId(id),
            name,
            new Money(100),
            new Distance(40),
            new Weight(100),
            specialRuleIds: [],
            sources:
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 155)
            ]);
    }
}
