using FiveEData.Rules.Expenses.FoodAndLodging;
using FiveEData.Rules.Expenses.FoodAndLodging.Serialization;

namespace FiveEData.Tests;

public sealed class FoodDrinkDefinitionLoaderTests
{
    [Fact]
    public void ValidDefinition_IsLoaded()
    {
        FoodDrinkDefinition definition =
            Assert.Single(
                FoodDrinkDefinitionLoader.LoadFromJson(
                    CreateJson()));

        Assert.Equal(
            "dnd5e2014.food-drink.bread",
            definition.Id.Value);
        Assert.Equal("Bread, loaf", definition.Name);
        Assert.Equal(2, definition.Cost.CopperPieces);
        Assert.Equal(
            FoodDrinkPricingUnit.Loaf,
            definition.PricingUnit);
    }

    [Fact]
    public void MissingCost_IsRejected()
    {
        string json = CreateJson().Replace(
            """
                "cost": {
                  "copperPieces": 2
                },
            """,
            string.Empty,
            StringComparison.Ordinal);

        Assert.Throws<InvalidDataException>(
            () => FoodDrinkDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void UnknownMember_IsRejected()
    {
        string json = CreateJson().Replace(
            """
                "specialRuleIds": [],
            """,
            """
                "unexpected": true,
                "specialRuleIds": [],
            """,
            StringComparison.Ordinal);

        Assert.Throws<InvalidDataException>(
            () => FoodDrinkDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void IntegerPricingUnit_IsRejected()
    {
        string json = CreateJson().Replace(
            """
                "pricingUnit": "Loaf",
            """,
            """
                "pricingUnit": 4,
            """,
            StringComparison.Ordinal);

        Assert.Throws<InvalidDataException>(
            () => FoodDrinkDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void UnknownPricingUnit_IsRejected()
    {
        string json = CreateJson().Replace(
            """
                "pricingUnit": "Loaf",
            """,
            """
                "pricingUnit": "Slice",
            """,
            StringComparison.Ordinal);

        Assert.Throws<InvalidDataException>(
            () => FoodDrinkDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void DuplicateIds_AreRejected()
    {
        string item = CreateJson().Trim();
        string objectJson = item[1..^1].Trim();

        string json = $"[{objectJson},{objectJson}]";

        Assert.Throws<InvalidDataException>(
            () => FoodDrinkDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void NullSpecialRuleIds_AreRejected()
    {
        string json = CreateJson().Replace(
            """
                "specialRuleIds": [],
            """,
            """
                "specialRuleIds": null,
            """,
            StringComparison.Ordinal);

        Assert.Throws<InvalidDataException>(
            () => FoodDrinkDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void NullSources_AreRejected()
    {
        const string json =
            """
            [
              {
                "id": "dnd5e2014.food-drink.bread",
                "name": "Bread, loaf",
                "cost": {
                  "copperPieces": 2
                },
                "pricingUnit": "Loaf",
                "specialRuleIds": [],
                "sources": null
              }
            ]
            """;

        Assert.Throws<InvalidDataException>(
            () => FoodDrinkDefinitionLoader.LoadFromJson(json));
    }

    private static string CreateJson()
    {
        return
            """
            [
              {
                "id": "dnd5e2014.food-drink.bread",
                "name": "Bread, loaf",
                "cost": {
                  "copperPieces": 2
                },
                "pricingUnit": "Loaf",
                "specialRuleIds": [],
                "sources": [
                  {
                    "documentId": "dnd5e2014.source.phb-first-printing",
                    "page": 158,
                    "section": "Food, Drink, and Lodging"
                  }
                ]
              }
            ]
            """;
    }
}
