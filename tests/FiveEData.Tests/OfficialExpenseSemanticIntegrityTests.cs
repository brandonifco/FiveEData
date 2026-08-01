using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common;
using FiveEData.Rules.Expenses;
using FiveEData.Rules.Expenses.FoodAndLodging;
using FiveEData.Rules.Expenses.FoodAndLodging.Serialization;
using FiveEData.Rules.Expenses.Lifestyles;
using FiveEData.Rules.Expenses.Lifestyles.Serialization;

namespace FiveEData.Tests;

public sealed class OfficialExpenseSemanticIntegrityTests
{
    [Fact]
    public void CanonicalExpenseDefinitions_HaveNoErrors()
    {
        ExpenseDefinitionSet expenses = LoadCanonical();

        Assert.Empty(
            OfficialExpenseSemanticValidator.Validate(
                expenses.FoodAndDrink,
                expenses.HospitalityCosts));
    }

    [Fact]
    public void MissingFoodDrinkDefinition_IsRejected()
    {
        ExpenseDefinitionSet canonical = LoadCanonical();

        ExpenseDefinitionSet altered =
            CreateExpenseSet(
                canonical,
                foodAndDrink:
                    canonical.FoodAndDrink
                        .Where(
                            definition =>
                                definition.Id.Value !=
                                "dnd5e2014.food-drink.wine-fine")
                        .ToArray());

        IReadOnlyList<string> errors =
            OfficialExpenseSemanticValidator.Validate(
                altered.FoodAndDrink,
                altered.HospitalityCosts);

        Assert.Contains(
            errors,
            error => error.Contains(
                "must contain exactly 8 definitions; found 7",
                StringComparison.Ordinal));

        Assert.Contains(
            errors,
            error => error.Contains(
                "is missing 'dnd5e2014.food-drink.wine-fine'",
                StringComparison.Ordinal));
    }

    [Fact]
    public void AlteredFoodDrinkSemantics_AreRejected()
    {
        ExpenseDefinitionSet canonical = LoadCanonical();

        FoodDrinkDefinition bread =
            canonical.FoodAndDrink.Single(
                definition =>
                    definition.Id.Value ==
                    "dnd5e2014.food-drink.bread");

        var alteredBread = new FoodDrinkDefinition(
            bread.Id,
            "Bread slice",
            new Money(3),
            FoodDrinkPricingUnit.Chunk,
            bread.SpecialRuleIds,
            bread.Sources);

        ExpenseDefinitionSet altered =
            CreateExpenseSet(
                canonical,
                foodAndDrink:
                    canonical.FoodAndDrink
                        .Select(
                            definition =>
                                definition.Id == bread.Id
                                    ? alteredBread
                                    : definition)
                        .ToArray());

        IReadOnlyList<string> errors =
            OfficialExpenseSemanticValidator.Validate(
                altered.FoodAndDrink,
                altered.HospitalityCosts);

        Assert.Contains(
            errors,
            error => error.Contains(
                "must be named 'Bread'",
                StringComparison.Ordinal));

        Assert.Contains(
            errors,
            error => error.Contains(
                "must cost 2 cp; found 3 cp",
                StringComparison.Ordinal));

        Assert.Contains(
            errors,
            error => error.Contains(
                "must use pricing unit 'Loaf'; found 'Chunk'",
                StringComparison.Ordinal));
    }

    [Fact]
    public void UnexpectedFoodDrinkDefinition_IsRejected()
    {
        ExpenseDefinitionSet canonical = LoadCanonical();

        FoodDrinkDefinition template =
            canonical.FoodAndDrink[0];

        var unexpected = new FoodDrinkDefinition(
            new FoodDrinkId(
                "dnd5e2014.food-drink.unexpected"),
            "Unexpected",
            new Money(1),
            FoodDrinkPricingUnit.Mug,
            template.SpecialRuleIds,
            template.Sources);

        ExpenseDefinitionSet altered =
            CreateExpenseSet(
                canonical,
                foodAndDrink:
                    canonical.FoodAndDrink
                        .Append(unexpected)
                        .ToArray());

        IReadOnlyList<string> errors =
            OfficialExpenseSemanticValidator.Validate(
                altered.FoodAndDrink,
                altered.HospitalityCosts);

        Assert.Contains(
            errors,
            error => error.Contains(
                "unexpected definition " +
                "'dnd5e2014.food-drink.unexpected'",
                StringComparison.Ordinal));
    }

    [Fact]
    public void AlteredHospitalityCosts_AreRejected()
    {
        ExpenseDefinitionSet canonical = LoadCanonical();

        LifestyleHospitalityCostDefinition modest =
            canonical.HospitalityCosts.Single(
                definition =>
                    definition.LifestyleId.Value ==
                    "dnd5e2014.lifestyle.modest");

        var alteredModest =
            new LifestyleHospitalityCostDefinition(
                modest.LifestyleId,
                new Money(51),
                new Money(31),
                modest.SpecialRuleIds,
                modest.Sources);

        ExpenseDefinitionSet altered =
            CreateExpenseSet(
                canonical,
                hospitalityCosts:
                    canonical.HospitalityCosts
                        .Select(
                            definition =>
                                definition.LifestyleId ==
                                modest.LifestyleId
                                    ? alteredModest
                                    : definition)
                        .ToArray());

        IReadOnlyList<string> errors =
            OfficialExpenseSemanticValidator.Validate(
                altered.FoodAndDrink,
                altered.HospitalityCosts);

        Assert.Contains(
            errors,
            error => error.Contains(
                "inn-stay cost of 50 cp per day; found 51 cp",
                StringComparison.Ordinal));

        Assert.Contains(
            errors,
            error => error.Contains(
                "meals cost of 30 cp per day; found 31 cp",
                StringComparison.Ordinal));
    }

    [Fact]
    public void WretchedHospitalityDefinition_IsRejected()
    {
        ExpenseDefinitionSet canonical = LoadCanonical();

        LifestyleHospitalityCostDefinition template =
            canonical.HospitalityCosts[0];

        var wretched =
            new LifestyleHospitalityCostDefinition(
                new LifestyleId(
                    "dnd5e2014.lifestyle.wretched"),
                new Money(1),
                new Money(1),
                template.SpecialRuleIds,
                template.Sources);

        ExpenseDefinitionSet altered =
            CreateExpenseSet(
                canonical,
                hospitalityCosts:
                    canonical.HospitalityCosts
                        .Append(wretched)
                        .ToArray());

        IReadOnlyList<string> errors =
            OfficialExpenseSemanticValidator.Validate(
                altered.FoodAndDrink,
                altered.HospitalityCosts);

        Assert.Contains(
            errors,
            error => error.Contains(
                "unexpected lifestyle " +
                "'dnd5e2014.lifestyle.wretched'",
                StringComparison.Ordinal));
    }

    [Fact]
    public void MissingHospitalityDefinition_IsRejected()
    {
        ExpenseDefinitionSet canonical = LoadCanonical();

        ExpenseDefinitionSet altered =
            CreateExpenseSet(
                canonical,
                hospitalityCosts:
                    canonical.HospitalityCosts
                        .Where(
                            definition =>
                                definition.LifestyleId.Value !=
                                "dnd5e2014.lifestyle.squalid")
                        .ToArray());

        IReadOnlyList<string> errors =
            OfficialExpenseSemanticValidator.Validate(
                altered.FoodAndDrink,
                altered.HospitalityCosts);

        Assert.Contains(
            errors,
            error => error.Contains(
                "must contain exactly 6 definitions; found 5",
                StringComparison.Ordinal));

        Assert.Contains(
            errors,
            error => error.Contains(
                "is missing lifestyle " +
                "'dnd5e2014.lifestyle.squalid'",
                StringComparison.Ordinal));
    }

    private static ExpenseDefinitionSet CreateExpenseSet(
        ExpenseDefinitionSet canonical,
        IReadOnlyList<FoodDrinkDefinition>? foodAndDrink = null,
        IReadOnlyList<
            LifestyleHospitalityCostDefinition>?
                hospitalityCosts = null)
    {
        return new ExpenseDefinitionSet(
            canonical.Lifestyles,
            foodAndDrink ?? canonical.FoodAndDrink,
            hospitalityCosts ?? canonical.HospitalityCosts);
    }

    private static ExpenseDefinitionSet LoadCanonical()
    {
        string root = FindRepositoryRoot();

        IReadOnlyList<LifestyleDefinition> lifestyles =
            LifestyleDefinitionLoader.LoadFromFile(
                Path.Combine(
                    root,
                    "Data",
                    "dnd5e2014",
                    "lifestyles.json"));

        IReadOnlyList<FoodDrinkDefinition> foodAndDrink =
            FoodDrinkDefinitionLoader.LoadFromFile(
                Path.Combine(
                    root,
                    "Data",
                    "dnd5e2014",
                    "food-drink.json"));

        IReadOnlyList<
            LifestyleHospitalityCostDefinition>
                hospitalityCosts =
                    LifestyleHospitalityCostDefinitionLoader
                        .LoadFromFile(
                            Path.Combine(
                                root,
                                "Data",
                                "dnd5e2014",
                                "lifestyle-hospitality-costs.json"));

        return new ExpenseDefinitionSet(
            lifestyles,
            foodAndDrink,
            hospitalityCosts);
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
}
