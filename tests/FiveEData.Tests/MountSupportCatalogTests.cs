using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment.MountSupport;

namespace FiveEData.Tests;

public sealed class MountSupportCatalogTests
{
    [Fact]
    public void Catalog_OrdersByStableIdAndProvidesLookup()
    {
        var catalog = new MountSupportCatalog(
            [
                Create("dnd5e2014.mount-support.z", "Z"),
                Create("dnd5e2014.mount-support.a", "A")
            ]);

        Assert.Equal(
            new[]
            {
                "dnd5e2014.mount-support.a",
                "dnd5e2014.mount-support.z"
            },
            catalog.All.Select(definition => definition.Id.Value).ToArray());

        Assert.Equal(
            "A",
            catalog.Get(
                new MountSupportId(
                    "dnd5e2014.mount-support.a")).Name);

        Assert.True(
            catalog.TryGet(
                new MountSupportId(
                    "dnd5e2014.mount-support.z"),
                out MountSupportDefinition? found));
        Assert.NotNull(found);
    }

    [Fact]
    public void Catalog_DefensivelySnapshotsInput()
    {
        var source = new List<MountSupportDefinition>
        {
            Create("dnd5e2014.mount-support.one", "One")
        };

        var catalog = new MountSupportCatalog(source);
        source.Add(Create("dnd5e2014.mount-support.two", "Two"));

        Assert.Single(catalog.All);
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new MountSupportCatalog(
                [
                    Create(
                        "dnd5e2014.mount-support.duplicate",
                        "One"),
                    Create(
                        "dnd5e2014.mount-support.duplicate",
                        "Two")
                ]));
    }

    [Fact]
    public void Catalog_RejectsDefaultIdAtTrustBoundary()
    {
        MountSupportDefinition definition = new(
            default,
            "Invalid",
            new Money(100),
            listedWeight: new Weight(1),
            specialRuleIds: [],
            sources:
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 157)
            ]);

        Assert.Throws<InvalidOperationException>(
            () => new MountSupportCatalog([definition]));
    }

    private static MountSupportDefinition Create(string id, string name)
    {
        return new MountSupportDefinition(
            new MountSupportId(id),
            name,
            new Money(100),
            listedWeight: new Weight(1),
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
