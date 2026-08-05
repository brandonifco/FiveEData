using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Creatures.Senses;

public sealed class SenseDefinition
{
    internal SenseDefinition(
        SenseId id,
        string name,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public SenseId Id { get; }
    public string Name { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
