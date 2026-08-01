using FiveEData.Rules.Catalog;

namespace FiveEData.Rules.Expenses;

public sealed class ExpenseCatalogs
{
    internal ExpenseCatalogs(
        LifestyleCatalog lifestyles,
        FoodDrinkCatalog foodAndDrink,
        LifestyleHospitalityCostCatalog hospitalityCosts)
    {
        ArgumentNullException.ThrowIfNull(lifestyles);
        ArgumentNullException.ThrowIfNull(foodAndDrink);
        ArgumentNullException.ThrowIfNull(hospitalityCosts);

        Lifestyles = lifestyles;
        FoodAndDrink = foodAndDrink;
        HospitalityCosts = hospitalityCosts;
    }

    public LifestyleCatalog Lifestyles { get; }
    public FoodDrinkCatalog FoodAndDrink { get; }

    public LifestyleHospitalityCostCatalog HospitalityCosts
    {
        get;
    }
}
