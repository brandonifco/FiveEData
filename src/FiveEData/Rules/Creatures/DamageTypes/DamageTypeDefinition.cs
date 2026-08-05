using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Creatures.DamageTypes;

public sealed class DamageTypeDefinition
{
    internal DamageTypeDefinition(
        DamageTypeId id,
        string name,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public DamageTypeId Id { get; }
    public string Name { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
