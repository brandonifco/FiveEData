using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Spells.MagicSchools;

public sealed class MagicSchoolDefinition
{
    internal MagicSchoolDefinition(
        MagicSchoolId id,
        string name,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public MagicSchoolId Id { get; }
    public string Name { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
