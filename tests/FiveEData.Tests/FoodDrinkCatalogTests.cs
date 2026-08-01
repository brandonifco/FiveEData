using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Expenses.FoodAndLodging;

namespace FiveEData.Tests;

public sealed class FoodDrinkCatalogTests
{
    [Fact]
    public void Catalog_OrdersByStableIdAndProvidesLookup()
    {
        var catalog = new FoodDrinkCatalog(
            [
                Create("dnd5e2014.food-drink.z", "Z"),
                Create("dnd5e2014.food-drink.a", "A")
            ]);

        Assert.Equal(
            new[]
            {
                "dnd5e2014.food-drink.a",
                "dnd5e2014.food-drink.z"
            },
            catalog.All
                .Select(definition => definition.Id.Value)
                .ToArray());

        Assert.Equal(
            "A",
            catalog.Get(
                new FoodDrinkId(
                    "dnd5e2014.food-drink.a")).Name);

        Assert.True(
            catalog.TryGet(
                new FoodDrinkId(
                    "dnd5e2014.food-drink.z"),
                out FoodDrinkDefinition? found));
        Assert.NotNull(found);
    }

    [Fact]
    public void Catalog_DefensivelySnapshotsInput()
    {
        var source = new List<FoodDrinkDefinition>
        {
            Create(
                "dnd5e2014.food-drink.one",
                "One")
        };

        var catalog = new FoodDrinkCatalog(source);

        source.Add(
            Create(
                "dnd5e2014.food-drink.two",
                "Two"));

        Assert.Single(catalog.All);
    }

    [Fact]
    public void Catalog_RejectsDuplicateIds()
    {
        Assert.Throws<ArgumentException>(
            () => new FoodDrinkCatalog(
                [
                    Create(
                        "dnd5e2014.food-drink.duplicate",
                        "One"),
                    Create(
                        "dnd5e2014.food-drink.duplicate",
                        "Two")
                ]));
    }

    [Fact]
    public void Catalog_RejectsInvalidDefinition()
    {
        FoodDrinkDefinition definition = new(
            default,
            "Invalid",
            new Money(10),
            FoodDrinkPricingUnit.Loaf,
            specialRuleIds: [],
            sources: [CreateSource()]);

        Assert.Throws<InvalidOperationException>(
            () => new FoodDrinkCatalog([definition]));
    }

    private static FoodDrinkDefinition Create(
        string id,
        string name)
    {
        return new FoodDrinkDefinition(
            new FoodDrinkId(id),
            name,
            new Money(10),
            FoodDrinkPricingUnit.Loaf,
            specialRuleIds: [],
            sources: [CreateSource()]);
    }

    private static SourceReference CreateSource()
    {
        return new SourceReference(
            new SourceDocumentId(
                "dnd5e2014.source.phb-first-printing"),
            page: 158);
    }
}
