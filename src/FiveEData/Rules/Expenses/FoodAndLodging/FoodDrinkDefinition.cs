using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Expenses.FoodAndLodging;

public sealed class FoodDrinkDefinition
{
    internal FoodDrinkDefinition(
        FoodDrinkId id,
        string name,
        Money cost,
        FoodDrinkPricingUnit pricingUnit,
        IEnumerable<RuleId> specialRuleIds,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(specialRuleIds);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        Cost = cost;
        PricingUnit = pricingUnit;
        SpecialRuleIds = Array.AsReadOnly(specialRuleIds.ToArray());
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public FoodDrinkId Id { get; }
    public string Name { get; }
    public Money Cost { get; }
    public FoodDrinkPricingUnit PricingUnit { get; }
    public IReadOnlyList<RuleId> SpecialRuleIds { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
