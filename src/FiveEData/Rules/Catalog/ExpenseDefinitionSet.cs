using FiveEData.Rules.Expenses.Lifestyles;

namespace FiveEData.Rules.Catalog;

internal sealed class ExpenseDefinitionSet
{
    public ExpenseDefinitionSet(
        IReadOnlyList<LifestyleDefinition> lifestyles)
    {
        ArgumentNullException.ThrowIfNull(lifestyles);

        Lifestyles = lifestyles;
    }

    public IReadOnlyList<LifestyleDefinition> Lifestyles { get; }
}
