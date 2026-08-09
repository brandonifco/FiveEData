using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Combat.CombatActions;

public sealed class CombatActionDefinition
{
    internal CombatActionDefinition(
        CombatActionId id,
        string name,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public CombatActionId Id { get; }
    public string Name { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
