using FiveEData.Rules.Catalog;

namespace FiveEData.Rules.Expenses;

public sealed class ExpenseCatalogs
{
    internal ExpenseCatalogs(
        LifestyleCatalog lifestyles)
    {
        ArgumentNullException.ThrowIfNull(lifestyles);

        Lifestyles = lifestyles;
    }

    public LifestyleCatalog Lifestyles { get; }
}
