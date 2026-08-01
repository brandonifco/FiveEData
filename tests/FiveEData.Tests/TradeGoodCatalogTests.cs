using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment.TradeGoods;

namespace FiveEData.Tests;

public sealed class TradeGoodCatalogTests
{
    [Fact]
    public void Catalog_OrdersByStableIdAndProvidesLookup()
    {
        var catalog = new TradeGoodCatalog(
            [
                Create("dnd5e2014.trade-good.z", "Z"),
                Create("dnd5e2014.trade-good.a", "A")
            ]);

        Assert.Equal(
            new[]
            {
                "dnd5e2014.trade-good.a",
                "dnd5e2014.trade-good.z"
            },
            catalog.All.Select(definition => definition.Id.Value).ToArray());

        Assert.Equal(
            "A",
            catalog.Get(
                new TradeGoodId(
                    "dnd5e2014.trade-good.a")).Name);

        Assert.True(
            catalog.TryGet(
                new TradeGoodId(
                    "dnd5e2014.trade-good.z"),
                out TradeGoodDefinition? found));
        Assert.NotNull(found);
    }

    [Fact]
    public void Catalog_DefensivelySnapshotsInput()
    {
        var source = new List<TradeGoodDefinition>
        {
            Create("dnd5e2014.trade-good.one", "One")
        };

        var catalog = new TradeGoodCatalog(source);
        source.Add(Create("dnd5e2014.trade-good.two", "Two"));

        Assert.Single(catalog.All);
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new TradeGoodCatalog(
                [
                    Create("dnd5e2014.trade-good.duplicate", "One"),
                    Create("dnd5e2014.trade-good.duplicate", "Two")
                ]));
    }

    [Fact]
    public void Catalog_RejectsDefaultIdAtTrustBoundary()
    {
        TradeGoodDefinition definition = new(
            default,
            "Invalid",
            new Money(100),
            new TradeGoodPricingBasis(1, TradeGoodUnit.Pound),
            specialRuleIds: [],
            sources:
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 157)
            ]);

        Assert.Throws<InvalidOperationException>(
            () => new TradeGoodCatalog([definition]));
    }

    private static TradeGoodDefinition Create(string id, string name)
    {
        return new TradeGoodDefinition(
            new TradeGoodId(id),
            name,
            new Money(100),
            new TradeGoodPricingBasis(1, TradeGoodUnit.Pound),
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
