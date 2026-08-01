using FiveEData.Rules.Expenses.FoodAndLodging;

namespace FiveEData.Tests;

public sealed class FoodDrinkRulesetIntegrationTests
{
    [Fact]
    public void EmbeddedRuleset_ExposesCanonicalFoodAndDrink()
    {
        Dnd5e2014Ruleset ruleset =
            Dnd5e2014Ruleset.Instance;

        Assert.Equal(
            8,
            ruleset.Expenses.FoodAndDrink.Count);

        FoodDrinkDefinition bread =
            ruleset.Expenses.FoodAndDrink.Get(
                new FoodDrinkId(
                    "dnd5e2014.food-drink.bread"));

        Assert.Equal("Bread", bread.Name);
        Assert.Equal(2, bread.Cost.CopperPieces);
        Assert.Equal(
            FoodDrinkPricingUnit.Loaf,
            bread.PricingUnit);

        FoodDrinkDefinition banquet =
            ruleset.Expenses.FoodAndDrink.Get(
                new FoodDrinkId(
                    "dnd5e2014.food-drink.banquet"));

        Assert.Equal(1000, banquet.Cost.CopperPieces);
        Assert.Equal(
            FoodDrinkPricingUnit.Person,
            banquet.PricingUnit);
    }
}
