using FiveEData.Rules.Expenses.FoodAndLodging;
using FiveEData.Rules.Expenses.Lifestyles;
using FiveEData.Rules.Expenses.Services;

namespace FiveEData.Rules.Catalog;

internal sealed class ExpenseDefinitionSet
{
    public ExpenseDefinitionSet(
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

        Lifestyles = lifestyles;
        FoodAndDrink = foodAndDrink;
        HospitalityCosts = hospitalityCosts;
        MundaneServices = mundaneServices;
    }

    public IReadOnlyList<LifestyleDefinition> Lifestyles { get; }
    public IReadOnlyList<FoodDrinkDefinition> FoodAndDrink { get; }

    public IReadOnlyList<
        LifestyleHospitalityCostDefinition> HospitalityCosts
    {
        get;
    }

    public IReadOnlyList<MundaneServiceDefinition>
        MundaneServices
    {
        get;
    }
}
