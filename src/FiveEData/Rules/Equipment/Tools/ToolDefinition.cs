using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Equipment.Tools;

public sealed class ToolDefinition
{
    internal ToolDefinition(
        ToolId id,
        string name,
        Money cost,
        Weight? weight,
        ToolFamilyId? familyId,
        IEnumerable<RuleId> specialRuleIds,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(specialRuleIds);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        Cost = cost;
        Weight = weight;
        FamilyId = familyId;
        SpecialRuleIds = Array.AsReadOnly(specialRuleIds.ToArray());
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public ToolId Id { get; }
    public string Name { get; }
    public Money Cost { get; }
    public Weight? Weight { get; }
    public ToolFamilyId? FamilyId { get; }
    public IReadOnlyList<RuleId> SpecialRuleIds { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
