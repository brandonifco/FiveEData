using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment.Tools;
using FiveEData.Rules.Catalog;

namespace FiveEData.Tests;

public sealed class ToolCatalogTests
{
    [Fact]
    public void Catalog_OrdersByStableIdAndProvidesLookup()
    {
        var catalog = new ToolCatalog(
            [
                Create("dnd5e2014.tool.z", "Z"),
                Create("dnd5e2014.tool.a", "A")
            ]);

        Assert.Equal(
            ["dnd5e2014.tool.a", "dnd5e2014.tool.z"],
            catalog.All.Select(definition => definition.Id.Value).ToArray());
        Assert.Equal("A", catalog.Get(new ToolId("dnd5e2014.tool.a")).Name);
        Assert.True(
            catalog.TryGet(
                new ToolId("dnd5e2014.tool.z"),
                out ToolDefinition? found));
        Assert.NotNull(found);
    }

    [Fact]
    public void Catalog_DefensivelySnapshotsInput()
    {
        var source = new List<ToolDefinition>
        {
            Create("dnd5e2014.tool.one", "One")
        };

        var catalog = new ToolCatalog(source);
        source.Add(Create("dnd5e2014.tool.two", "Two"));

        Assert.Single(catalog.All);
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new ToolCatalog(
                [
                    Create("dnd5e2014.tool.duplicate", "One"),
                    Create("dnd5e2014.tool.duplicate", "Two")
                ]));
    }

    [Fact]
    public void Catalog_RejectsDefaultIdAtTrustBoundary()
    {
        ToolDefinition definition = new(
            default,
            "Invalid",
            new Money(100),
            new Weight(1),
            familyId: null,
            specialRuleIds: [],
            sources:
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 154)
            ]);

        Assert.Throws<InvalidOperationException>(
            () => new ToolCatalog([definition]));
    }

    private static ToolDefinition Create(string id, string name)
    {
        return new ToolDefinition(
            new ToolId(id),
            name,
            new Money(100),
            new Weight(1),
            familyId: null,
            specialRuleIds: [],
            sources:
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 154)
            ]);
    }
}
