using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Equipment.AdventuringGear;

public sealed class AdventuringGearDefinition
{
    internal AdventuringGearDefinition(
        AdventuringGearId id,
        string name,
        Money cost,
        ListedWeight? listedWeight,
        IEnumerable<RuleId> specialRuleIds,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(specialRuleIds);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        Cost = cost;
        ListedWeight = listedWeight;
        SpecialRuleIds = Array.AsReadOnly(specialRuleIds.ToArray());
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public AdventuringGearId Id { get; }
    public string Name { get; }
    public Money Cost { get; }
    public ListedWeight? ListedWeight { get; }
    public IReadOnlyList<RuleId> SpecialRuleIds { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
