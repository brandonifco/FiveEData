using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Creatures.Abilities;

public sealed class AbilityDefinition
{
    internal AbilityDefinition(
        AbilityId id,
        string name,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public AbilityId Id { get; }
    public string Name { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
