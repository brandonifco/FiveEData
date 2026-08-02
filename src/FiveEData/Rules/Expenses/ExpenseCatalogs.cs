using FiveEData.Rules.Catalog;

namespace FiveEData.Rules.Expenses;

public sealed class ExpenseCatalogs
{
    internal ExpenseCatalogs(
        LifestyleCatalog lifestyles,
        FoodDrinkCatalog foodAndDrink,
        LifestyleHospitalityCostCatalog hospitalityCosts,
        MundaneServiceCatalog mundaneServices)
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

    public LifestyleCatalog Lifestyles { get; }
    public FoodDrinkCatalog FoodAndDrink { get; }

    public LifestyleHospitalityCostCatalog HospitalityCosts
    {
        get;
    }

    public MundaneServiceCatalog MundaneServices { get; }
}
