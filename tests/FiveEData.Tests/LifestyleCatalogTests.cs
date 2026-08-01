using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Expenses.Lifestyles;

namespace FiveEData.Tests;

public sealed class LifestyleCatalogTests
{
    [Fact]
    public void Catalog_OrdersByStableIdAndProvidesLookup()
    {
        var catalog = new LifestyleCatalog(
            [
                Create("dnd5e2014.lifestyle.z", "Z"),
                Create("dnd5e2014.lifestyle.a", "A")
            ]);

        Assert.Equal(
            new[]
            {
                "dnd5e2014.lifestyle.a",
                "dnd5e2014.lifestyle.z"
            },
            catalog.All
                .Select(definition => definition.Id.Value)
                .ToArray());

        Assert.Equal(
            "A",
            catalog.Get(
                new LifestyleId(
                    "dnd5e2014.lifestyle.a")).Name);

        Assert.True(
            catalog.TryGet(
                new LifestyleId(
                    "dnd5e2014.lifestyle.z"),
                out LifestyleDefinition? found));
        Assert.NotNull(found);
    }

    [Fact]
    public void Catalog_DefensivelySnapshotsInput()
    {
        var source = new List<LifestyleDefinition>
        {
            Create(
                "dnd5e2014.lifestyle.one",
                "One")
        };

        var catalog = new LifestyleCatalog(source);

        source.Add(
            Create(
                "dnd5e2014.lifestyle.two",
                "Two"));

        Assert.Single(catalog.All);
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new LifestyleCatalog(
                [
                    Create(
                        "dnd5e2014.lifestyle.duplicate",
                        "One"),
                    Create(
                        "dnd5e2014.lifestyle.duplicate",
                        "Two")
                ]));
    }

    [Fact]
    public void Catalog_RejectsDefaultIdAtTrustBoundary()
    {
        LifestyleDefinition definition = new(
            default,
            "Invalid",
            new ListedCost(
                new Money(100),
                ListedCostKind.Exact),
            specialRuleIds: [],
            sources:
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 157)
            ]);

        Assert.Throws<InvalidOperationException>(
            () => new LifestyleCatalog([definition]));
    }

    private static LifestyleDefinition Create(
        string id,
        string name)
    {
        return new LifestyleDefinition(
            new LifestyleId(id),
            name,
            new ListedCost(
                new Money(100),
                ListedCostKind.Exact),
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
