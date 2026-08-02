using FiveEData.Rules.Expenses.FoodAndLodging;
using FiveEData.Rules.Expenses.Lifestyles;
using FiveEData.Rules.Expenses.Services;

namespace FiveEData.Rules.Expenses;

internal static class OfficialExpenseSemanticValidator
{
    private static readonly FoodDrinkExpectation[]
        OfficialFoodDrinkExpectations =
        [
            new(
                new FoodDrinkId(
                    "dnd5e2014.food-drink.ale-gallon"),
                "Ale",
                20,
                FoodDrinkPricingUnit.Gallon),
            new(
                new FoodDrinkId(
                    "dnd5e2014.food-drink.ale-mug"),
                "Ale",
                4,
                FoodDrinkPricingUnit.Mug),
            new(
                new FoodDrinkId(
                    "dnd5e2014.food-drink.banquet"),
                "Banquet",
                1000,
                FoodDrinkPricingUnit.Person),
            new(
                new FoodDrinkId(
                    "dnd5e2014.food-drink.bread"),
                "Bread",
                2,
                FoodDrinkPricingUnit.Loaf),
            new(
                new FoodDrinkId(
                    "dnd5e2014.food-drink.cheese"),
                "Cheese",
                10,
                FoodDrinkPricingUnit.Hunk),
            new(
                new FoodDrinkId(
                    "dnd5e2014.food-drink.meat"),
                "Meat",
                30,
                FoodDrinkPricingUnit.Chunk),
            new(
                new FoodDrinkId(
                    "dnd5e2014.food-drink.wine-common"),
                "Wine, common",
                20,
                FoodDrinkPricingUnit.Pitcher),
            new(
                new FoodDrinkId(
                    "dnd5e2014.food-drink.wine-fine"),
                "Wine, fine",
                1000,
                FoodDrinkPricingUnit.Bottle)
        ];

    private static readonly HospitalityCostExpectation[]
        OfficialHospitalityCostExpectations =
        [
            new(
                new LifestyleId(
                    "dnd5e2014.lifestyle.squalid"),
                7,
                3),
            new(
                new LifestyleId(
                    "dnd5e2014.lifestyle.poor"),
                10,
                6),
            new(
                new LifestyleId(
                    "dnd5e2014.lifestyle.modest"),
                50,
                30),
            new(
                new LifestyleId(
                    "dnd5e2014.lifestyle.comfortable"),
                80,
                50),
            new(
                new LifestyleId(
                    "dnd5e2014.lifestyle.wealthy"),
                200,
                80),
            new(
                new LifestyleId(
                    "dnd5e2014.lifestyle.aristocratic"),
                400,
                200)
        ];

    public static IReadOnlyList<string> Validate(
        IReadOnlyList<LifestyleDefinition> lifestyles,
        IReadOnlyList<FoodDrinkDefinition> foodAndDrink,
        IReadOnlyList<LifestyleHospitalityCostDefinition>
            hospitalityCosts,
        IReadOnlyList<MundaneServiceDefinition>
            mundaneServices)
    {
        ArgumentNullException.ThrowIfNull(lifestyles);
        ArgumentNullException.ThrowIfNull(foodAndDrink);
        ArgumentNullException.ThrowIfNull(hospitalityCosts);
        ArgumentNullException.ThrowIfNull(mundaneServices);

        var errors = new List<string>();

        errors.AddRange(
            OfficialLifestyleSemanticValidator.Validate(
                lifestyles));

        ValidateOfficialFoodDrinkSemantics(
            foodAndDrink,
            errors);

        ValidateOfficialHospitalitySemantics(
            hospitalityCosts,
            errors);

        errors.AddRange(
            OfficialMundaneServiceSemanticValidator.Validate(
                mundaneServices));

        return errors;
    }

    private static void ValidateOfficialFoodDrinkSemantics(
        IReadOnlyList<FoodDrinkDefinition> definitions,
        ICollection<string> errors)
    {
        if (definitions.Count !=
            OfficialFoodDrinkExpectations.Length)
        {
            errors.Add(
                "Official food-and-drink catalog must contain " +
                $"exactly {OfficialFoodDrinkExpectations.Length} " +
                $"definitions; found {definitions.Count}.");
        }

        var byId =
            new Dictionary<
                FoodDrinkId,
                FoodDrinkDefinition>();

        foreach (FoodDrinkDefinition definition in definitions)
        {
            if (!byId.TryAdd(definition.Id, definition))
            {
                errors.Add(
                    "Official food-and-drink catalog contains " +
                    $"duplicate ID '{definition.Id}'.");
            }
        }

        HashSet<FoodDrinkId> expectedIds =
            OfficialFoodDrinkExpectations
                .Select(expectation => expectation.Id)
                .ToHashSet();

        foreach (
            FoodDrinkExpectation expectation
            in OfficialFoodDrinkExpectations)
        {
            if (!byId.TryGetValue(
                    expectation.Id,
                    out FoodDrinkDefinition? definition))
            {
                errors.Add(
                    "Official food-and-drink catalog is missing " +
                    $"'{expectation.Id}'.");
                continue;
            }

            if (!string.Equals(
                    definition.Name,
                    expectation.Name,
                    StringComparison.Ordinal))
            {
                errors.Add(
                    $"Official food and drink '{expectation.Id}' " +
                    $"must be named '{expectation.Name}'; found " +
                    $"'{definition.Name}'.");
            }

            if (definition.Cost.CopperPieces !=
                expectation.CopperPieces)
            {
                errors.Add(
                    $"Official food and drink '{expectation.Id}' " +
                    $"must cost {expectation.CopperPieces} cp; " +
                    $"found {definition.Cost.CopperPieces} cp.");
            }

            if (definition.PricingUnit !=
                expectation.PricingUnit)
            {
                errors.Add(
                    $"Official food and drink '{expectation.Id}' " +
                    $"must use pricing unit " +
                    $"'{expectation.PricingUnit}'; found " +
                    $"'{definition.PricingUnit}'.");
            }
        }

        foreach (
            FoodDrinkId unexpectedId
            in byId.Keys
                .Where(id => !expectedIds.Contains(id))
                .OrderBy(
                    id => id.Value,
                    StringComparer.Ordinal))
        {
            errors.Add(
                "Official food-and-drink catalog contains " +
                $"unexpected definition '{unexpectedId}'.");
        }
    }

    private static void ValidateOfficialHospitalitySemantics(
        IReadOnlyList<
            LifestyleHospitalityCostDefinition> definitions,
        ICollection<string> errors)
    {
        if (definitions.Count !=
            OfficialHospitalityCostExpectations.Length)
        {
            errors.Add(
                "Official hospitality-cost catalog must contain " +
                $"exactly {OfficialHospitalityCostExpectations.Length} " +
                $"definitions; found {definitions.Count}.");
        }

        var byLifestyleId =
            new Dictionary<
                LifestyleId,
                LifestyleHospitalityCostDefinition>();

        foreach (
            LifestyleHospitalityCostDefinition definition
            in definitions)
        {
            if (!byLifestyleId.TryAdd(
                    definition.LifestyleId,
                    definition))
            {
                errors.Add(
                    "Official hospitality-cost catalog contains " +
                    "duplicate lifestyle ID " +
                    $"'{definition.LifestyleId}'.");
            }
        }

        HashSet<LifestyleId> expectedIds =
            OfficialHospitalityCostExpectations
                .Select(expectation => expectation.LifestyleId)
                .ToHashSet();

        foreach (
            HospitalityCostExpectation expectation
            in OfficialHospitalityCostExpectations)
        {
            if (!byLifestyleId.TryGetValue(
                    expectation.LifestyleId,
                    out LifestyleHospitalityCostDefinition?
                        definition))
            {
                errors.Add(
                    "Official hospitality-cost catalog is missing " +
                    $"lifestyle '{expectation.LifestyleId}'.");
                continue;
            }

            if (definition.InnStayCostPerDay.CopperPieces !=
                expectation.InnStayCopperPieces)
            {
                errors.Add(
                    "Official hospitality cost for lifestyle " +
                    $"'{expectation.LifestyleId}' must have an " +
                    $"inn-stay cost of " +
                    $"{expectation.InnStayCopperPieces} cp per day; " +
                    $"found " +
                    $"{definition.InnStayCostPerDay.CopperPieces} cp.");
            }

            if (definition.MealsCostPerDay.CopperPieces !=
                expectation.MealsCopperPieces)
            {
                errors.Add(
                    "Official hospitality cost for lifestyle " +
                    $"'{expectation.LifestyleId}' must have a " +
                    $"meals cost of " +
                    $"{expectation.MealsCopperPieces} cp per day; " +
                    $"found " +
                    $"{definition.MealsCostPerDay.CopperPieces} cp.");
            }
        }

        foreach (
            LifestyleId unexpectedId
            in byLifestyleId.Keys
                .Where(id => !expectedIds.Contains(id))
                .OrderBy(
                    id => id.Value,
                    StringComparer.Ordinal))
        {
            errors.Add(
                "Official hospitality-cost catalog contains " +
                $"unexpected lifestyle '{unexpectedId}'.");
        }
    }

    private readonly record struct FoodDrinkExpectation(
        FoodDrinkId Id,
        string Name,
        long CopperPieces,
        FoodDrinkPricingUnit PricingUnit);

    private readonly record struct HospitalityCostExpectation(
        LifestyleId LifestyleId,
        long InnStayCopperPieces,
        long MealsCopperPieces);

}
