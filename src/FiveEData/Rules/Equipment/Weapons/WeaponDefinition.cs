using System.Collections.Frozen;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment.Ammunition;

namespace FiveEData.Rules.Equipment.Weapons;

public sealed class WeaponDefinition
{
    internal WeaponDefinition(
        WeaponId id,
        string name,
        WeaponProficiencyCategory proficiencyCategory,
        WeaponUsageCategory usageCategory,
        Money? cost,
        Weight? weight,
        WeaponDamage? damage,
        IEnumerable<WeaponProperty> properties,
        WeaponRange? range,
        DiceExpression? versatileDamage,
        AmmunitionTypeId? ammunitionTypeId,
        IEnumerable<RuleId> specialRuleIds,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(properties);
        ArgumentNullException.ThrowIfNull(specialRuleIds);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        ProficiencyCategory = proficiencyCategory;
        UsageCategory = usageCategory;
        Cost = cost;
        Weight = weight;
        Damage = damage;
        Properties = properties.ToFrozenSet();
        Range = range;
        VersatileDamage = versatileDamage;
        AmmunitionTypeId = ammunitionTypeId;
        SpecialRuleIds = specialRuleIds.ToFrozenSet();
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public WeaponId Id { get; }
    public string Name { get; }
    public WeaponProficiencyCategory ProficiencyCategory { get; }
    public WeaponUsageCategory UsageCategory { get; }
    public Money? Cost { get; }
    public Weight? Weight { get; }
    public WeaponDamage? Damage { get; }
    public IReadOnlySet<WeaponProperty> Properties { get; }
    public WeaponRange? Range { get; }
    public DiceExpression? VersatileDamage { get; }
    public AmmunitionTypeId? AmmunitionTypeId { get; }
    public IReadOnlySet<RuleId> SpecialRuleIds { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
