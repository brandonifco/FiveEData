using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Creatures.Sizes;

public sealed class CreatureSizeDefinition
{
    internal CreatureSizeDefinition(
        CreatureSizeId id,
        string name,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public CreatureSizeId Id { get; }
    public string Name { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
