using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Expenses.Services;

public sealed class MundaneServiceDefinition
{
    internal MundaneServiceDefinition(
        MundaneServiceId id,
        string name,
        ListedCost cost,
        ServicePricingUnit pricingUnit,
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
        SpecialRuleIds =
            Array.AsReadOnly(specialRuleIds.ToArray());
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public MundaneServiceId Id { get; }
    public string Name { get; }
    public ListedCost Cost { get; }
    public ServicePricingUnit PricingUnit { get; }
    public IReadOnlyList<RuleId> SpecialRuleIds { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
