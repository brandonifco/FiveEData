using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Expenses.Lifestyles;

public sealed class LifestyleDefinition
{
    internal LifestyleDefinition(
        LifestyleId id,
        string name,
        ListedCost? dailyCost,
        IEnumerable<RuleId> specialRuleIds,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(specialRuleIds);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        DailyCost = dailyCost;
        SpecialRuleIds = Array.AsReadOnly(specialRuleIds.ToArray());
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public LifestyleId Id { get; }
    public string Name { get; }
    public ListedCost? DailyCost { get; }
    public IReadOnlyList<RuleId> SpecialRuleIds { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
