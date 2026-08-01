using FiveEData.Rules.Expenses.FoodAndLodging;
using FiveEData.Rules.Expenses.Lifestyles;

namespace FiveEData.Rules.Catalog;

internal sealed class ExpenseDefinitionSet
{
    public ExpenseDefinitionSet(
        IReadOnlyList<LifestyleDefinition> lifestyles,
        IReadOnlyList<FoodDrinkDefinition> foodAndDrink,
        IReadOnlyList<LifestyleHospitalityCostDefinition>
            hospitalityCosts)
    {
        ArgumentNullException.ThrowIfNull(lifestyles);
        ArgumentNullException.ThrowIfNull(foodAndDrink);
        ArgumentNullException.ThrowIfNull(hospitalityCosts);

        Lifestyles = lifestyles;
        FoodAndDrink = foodAndDrink;
        HospitalityCosts = hospitalityCosts;
    }

    public IReadOnlyList<LifestyleDefinition> Lifestyles { get; }
    public IReadOnlyList<FoodDrinkDefinition> FoodAndDrink { get; }

    public IReadOnlyList<
        LifestyleHospitalityCostDefinition> HospitalityCosts
    {
        get;
    }
}
