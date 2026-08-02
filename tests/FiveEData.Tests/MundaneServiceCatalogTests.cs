using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Expenses.Services;

namespace FiveEData.Tests;

public sealed class MundaneServiceCatalogTests
{
    [Fact]
    public void Catalog_OrdersByStableIdAndProvidesLookup()
    {
        var catalog = new MundaneServiceCatalog(
            [
                Create(
                    "dnd5e2014.mundane-service.z",
                    "Z"),
                Create(
                    "dnd5e2014.mundane-service.a",
                    "A")
            ]);

        Assert.Equal(
            new[]
            {
                "dnd5e2014.mundane-service.a",
                "dnd5e2014.mundane-service.z"
            },
            catalog.All
                .Select(definition => definition.Id.Value)
                .ToArray());

        Assert.Equal(
            "A",
            catalog.Get(
                new MundaneServiceId(
                    "dnd5e2014.mundane-service.a")).Name);

        Assert.True(
            catalog.TryGet(
                new MundaneServiceId(
                    "dnd5e2014.mundane-service.z"),
                out MundaneServiceDefinition? found));
        Assert.NotNull(found);
    }

    [Fact]
    public void Catalog_DefensivelySnapshotsInput()
    {
        var source = new List<MundaneServiceDefinition>
        {
            Create(
                "dnd5e2014.mundane-service.one",
                "One")
        };

        var catalog = new MundaneServiceCatalog(source);

        source.Add(
            Create(
                "dnd5e2014.mundane-service.two",
                "Two"));

        Assert.Single(catalog.All);
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new MundaneServiceCatalog(
                [
                    Create(
                        "dnd5e2014.mundane-service.duplicate",
                        "One"),
                    Create(
                        "dnd5e2014.mundane-service.duplicate",
                        "Two")
                ]));
    }

    [Fact]
    public void Catalog_RejectsDefaultIdAtTrustBoundary()
    {
        var definition = new MundaneServiceDefinition(
            default,
            "Invalid",
            new ListedCost(
                new Money(100),
                ListedCostKind.Exact),
            ServicePricingUnit.Day,
            specialRuleIds: [],
            sources:
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 159)
            ]);

        Assert.Throws<InvalidOperationException>(
            () => new MundaneServiceCatalog([definition]));
    }

    private static MundaneServiceDefinition Create(
        string id,
        string name)
    {
        return new MundaneServiceDefinition(
            new MundaneServiceId(id),
            name,
            new ListedCost(
                new Money(100),
                ListedCostKind.Exact),
            ServicePricingUnit.Day,
            specialRuleIds: [],
            sources:
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 159)
            ]);
    }
}
