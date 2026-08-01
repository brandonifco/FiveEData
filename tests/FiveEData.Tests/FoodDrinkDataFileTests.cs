using FiveEData.Rules.Common;
using FiveEData.Rules.Expenses.FoodAndLodging;
using FiveEData.Rules.Expenses.FoodAndLodging.Serialization;

namespace FiveEData.Tests;

public sealed class FoodDrinkDataFileTests
{
    private const string IncludedRuleId =
        "dnd5e2014.expense-rule." +
        "food-drink-lodging-included-in-lifestyle";

    private const string ExpectedSection =
        "Chapter 5: Equipment — Expenses — " +
        "Food, Drink, and Lodging";

    [Fact]
    public void CanonicalFile_ContainsExactlyEightRows()
    {
        IReadOnlyList<FoodDrinkDefinition> definitions =
            LoadCanonical();

        Assert.Equal(8, definitions.Count);
        Assert.Equal(
            8,
            definitions
                .Select(definition => definition.Id)
                .Distinct()
                .Count());
    }

    [Fact]
    public void CanonicalFile_MatchesFirstPrintingTable()
    {
        IReadOnlyDictionary<FoodDrinkId, FoodDrinkDefinition> actual =
            LoadCanonical().ToDictionary(
                definition => definition.Id);

        foreach (ExpectedFoodDrink expected in Expected)
        {
            FoodDrinkDefinition definition =
                actual[new FoodDrinkId(expected.Id)];

            Assert.Equal(expected.Name, definition.Name);
            Assert.Equal(
                expected.CopperPieces,
                definition.Cost.CopperPieces);
            Assert.Equal(
                expected.PricingUnit,
                definition.PricingUnit);

            Assert.Equal(
                [new RuleId(IncludedRuleId)],
                definition.SpecialRuleIds);

            var source = Assert.Single(definition.Sources);

            Assert.Equal(158, source.Page);
            Assert.Equal(ExpectedSection, source.Section);
        }
    }

    [Fact]
    public void CanonicalFile_UsesEveryApprovedPricingUnitExactlyOnce()
    {
        FoodDrinkPricingUnit[] actual =
            LoadCanonical()
                .Select(definition => definition.PricingUnit)
                .OrderBy(unit => unit)
                .ToArray();

        FoodDrinkPricingUnit[] expected =
            Enum.GetValues<FoodDrinkPricingUnit>()
                .OrderBy(unit => unit)
                .ToArray();

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CanonicalFile_AssociatesEveryRowWithLifestyleInclusionRule()
    {
        Assert.All(
            LoadCanonical(),
            definition =>
                Assert.Equal(
                    [new RuleId(IncludedRuleId)],
                    definition.SpecialRuleIds));
    }

    private static IReadOnlyList<FoodDrinkDefinition>
        LoadCanonical()
    {
        return FoodDrinkDefinitionLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "food-drink.json"));
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory =
            new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(
                    Path.Combine(
                        directory.FullName,
                        "FiveEData.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not locate the FiveEData repository root.");
    }

    private sealed record ExpectedFoodDrink(
        string Id,
        string Name,
        long CopperPieces,
        FoodDrinkPricingUnit PricingUnit);

    private static readonly ExpectedFoodDrink[] Expected =
    [
        new(
            "dnd5e2014.food-drink.ale-gallon",
            "Ale",
            20,
            FoodDrinkPricingUnit.Gallon),
        new(
            "dnd5e2014.food-drink.ale-mug",
            "Ale",
            4,
            FoodDrinkPricingUnit.Mug),
        new(
            "dnd5e2014.food-drink.banquet",
            "Banquet",
            1000,
            FoodDrinkPricingUnit.Person),
        new(
            "dnd5e2014.food-drink.bread",
            "Bread",
            2,
            FoodDrinkPricingUnit.Loaf),
        new(
            "dnd5e2014.food-drink.cheese",
            "Cheese",
            10,
            FoodDrinkPricingUnit.Hunk),
        new(
            "dnd5e2014.food-drink.meat",
            "Meat",
            30,
            FoodDrinkPricingUnit.Chunk),
        new(
            "dnd5e2014.food-drink.wine-common",
            "Wine, common",
            20,
            FoodDrinkPricingUnit.Pitcher),
        new(
            "dnd5e2014.food-drink.wine-fine",
            "Wine, fine",
            1000,
            FoodDrinkPricingUnit.Bottle)
    ];
}
