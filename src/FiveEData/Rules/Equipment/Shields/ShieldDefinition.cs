using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Equipment.Shields;

public sealed class ShieldDefinition
{
    internal ShieldDefinition(
        ShieldId id,
        string name,
        Money cost,
        Weight weight,
        int armorClassBonus,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        Cost = cost;
        Weight = weight;
        ArmorClassBonus = armorClassBonus;
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public ShieldId Id { get; }
    public string Name { get; }
    public Money Cost { get; }
    public Weight Weight { get; }
    public int ArmorClassBonus { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
