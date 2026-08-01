using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Equipment.Mounts;

public sealed class MountDefinition
{
    internal MountDefinition(
        MountId id,
        string name,
        Money cost,
        Distance speed,
        Weight baseCarryingCapacity,
        IEnumerable<RuleId> specialRuleIds,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(specialRuleIds);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        Cost = cost;
        Speed = speed;
        BaseCarryingCapacity = baseCarryingCapacity;
        SpecialRuleIds = Array.AsReadOnly(specialRuleIds.ToArray());
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public MountId Id { get; }
    public string Name { get; }
    public Money Cost { get; }
    public Distance Speed { get; }
    public Weight BaseCarryingCapacity { get; }
    public IReadOnlyList<RuleId> SpecialRuleIds { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
