using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Classes.Metamagic;

public sealed class MetamagicOptionDefinition
{
    internal MetamagicOptionDefinition(
        MetamagicOptionId id,
        string name,
        int? fixedSorceryPointCost,
        bool costEqualsSpellLevelWithCantripMinimum,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        FixedSorceryPointCost = fixedSorceryPointCost;
        CostEqualsSpellLevelWithCantripMinimum =
            costEqualsSpellLevelWithCantripMinimum;
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public MetamagicOptionId Id { get; }
    public string Name { get; }
    public int? FixedSorceryPointCost { get; }
    public bool CostEqualsSpellLevelWithCantripMinimum { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
