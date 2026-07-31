using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Equipment.Ammunition;

public sealed class AmmunitionDefinition
{
    internal AmmunitionDefinition(
        AmmunitionTypeId id,
        string name,
        int bundleQuantity,
        Money cost,
        Weight weight,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        BundleQuantity = bundleQuantity;
        Cost = cost;
        Weight = weight;
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public AmmunitionTypeId Id { get; }
    public string Name { get; }
    public int BundleQuantity { get; }
    public Money Cost { get; }
    public Weight Weight { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
