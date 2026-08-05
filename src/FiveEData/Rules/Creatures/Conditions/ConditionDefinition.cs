using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Creatures.Conditions;

public sealed class ConditionDefinition
{
    internal ConditionDefinition(
        ConditionId id,
        string name,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public ConditionId Id { get; }
    public string Name { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
