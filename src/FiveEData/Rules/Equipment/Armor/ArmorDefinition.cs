using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Equipment.Armor;

public sealed class ArmorDefinition
{
    internal ArmorDefinition(
        ArmorId id,
        string name,
        ArmorCategory category,
        Money cost,
        Weight weight,
        ArmorClassFormula armorClass,
        int? minimumStrengthForFullSpeed,
        bool imposesStealthDisadvantage,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        Category = category;
        Cost = cost;
        Weight = weight;
        ArmorClass = armorClass;
        MinimumStrengthForFullSpeed = minimumStrengthForFullSpeed;
        ImposesStealthDisadvantage = imposesStealthDisadvantage;
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public ArmorId Id { get; }
    public string Name { get; }
    public ArmorCategory Category { get; }
    public Money Cost { get; }
    public Weight Weight { get; }
    public ArmorClassFormula ArmorClass { get; }
    public int? MinimumStrengthForFullSpeed { get; }
    public bool ImposesStealthDisadvantage { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
