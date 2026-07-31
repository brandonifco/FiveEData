using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment.Tools;

namespace FiveEData.Tests;

public sealed class ToolFamilyCatalogTests
{
    [Fact]
    public void Catalog_OrdersByStableIdAndProvidesLookup()
    {
        var catalog = new ToolFamilyCatalog(
            [
                Create("dnd5e2014.tool-family.z", "Z"),
                Create("dnd5e2014.tool-family.a", "A")
            ]);

        Assert.Equal(
            ["dnd5e2014.tool-family.a", "dnd5e2014.tool-family.z"],
            catalog.All.Select(definition => definition.Id.Value).ToArray());
        Assert.Equal(
            "A",
            catalog.Get(new ToolFamilyId("dnd5e2014.tool-family.a")).Name);
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new ToolFamilyCatalog(
                [
                    Create("dnd5e2014.tool-family.duplicate", "One"),
                    Create("dnd5e2014.tool-family.duplicate", "Two")
                ]));
    }

    private static ToolFamilyDefinition Create(string id, string name)
    {
        return new ToolFamilyDefinition(
            new ToolFamilyId(id),
            name,
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
