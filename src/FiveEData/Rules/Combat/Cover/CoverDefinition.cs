using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Combat.Cover;

public sealed class CoverDefinition
{
    internal CoverDefinition(
        CoverId id,
        string name,
        int? armorClassBonus,
        int? dexteritySavingThrowBonus,
        bool preventsBeingTargeted,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        ArmorClassBonus = armorClassBonus;
        DexteritySavingThrowBonus = dexteritySavingThrowBonus;
        PreventsBeingTargeted = preventsBeingTargeted;
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public CoverId Id { get; }
    public string Name { get; }
    public int? ArmorClassBonus { get; }
    public int? DexteritySavingThrowBonus { get; }
    public bool PreventsBeingTargeted { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
