using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Expenses.Lifestyles;

namespace FiveEData.Rules.Expenses.FoodAndLodging;

public sealed class LifestyleHospitalityCostDefinition
{
    internal LifestyleHospitalityCostDefinition(
        LifestyleId lifestyleId,
        Money innStayCostPerDay,
        Money mealsCostPerDay,
        IEnumerable<RuleId> specialRuleIds,
        IEnumerable<SourceReference> sources)
    {
        ArgumentNullException.ThrowIfNull(specialRuleIds);
        ArgumentNullException.ThrowIfNull(sources);

        LifestyleId = lifestyleId;
        InnStayCostPerDay = innStayCostPerDay;
        MealsCostPerDay = mealsCostPerDay;
        SpecialRuleIds =
            Array.AsReadOnly(specialRuleIds.ToArray());
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public LifestyleId LifestyleId { get; }
    public Money InnStayCostPerDay { get; }
    public Money MealsCostPerDay { get; }
    public IReadOnlyList<RuleId> SpecialRuleIds { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
