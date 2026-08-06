using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Classes.ExtraAttack;

public sealed class ExtraAttackProgressionDefinition
{
    internal ExtraAttackProgressionDefinition(
        ExtraAttackProgressionId id,
        string name,
        IEnumerable<ExtraAttackGrant> grants,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(grants);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        Grants = Array.AsReadOnly(grants.ToArray());
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public ExtraAttackProgressionId Id { get; }
    public string Name { get; }
    public IReadOnlyList<ExtraAttackGrant> Grants { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
