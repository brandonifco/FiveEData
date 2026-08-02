using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;

namespace FiveEData.Rules.Creatures.Skills;

public sealed class SkillDefinition
{
    internal SkillDefinition(
        SkillId id,
        string name,
        AbilityId normallyAssociatedAbilityId,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        NormallyAssociatedAbilityId =
            normallyAssociatedAbilityId;
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public SkillId Id { get; }
    public string Name { get; }
    public AbilityId NormallyAssociatedAbilityId { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
