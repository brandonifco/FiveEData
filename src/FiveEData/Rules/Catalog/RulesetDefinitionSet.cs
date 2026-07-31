using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment.Ammunition;
using FiveEData.Rules.Equipment.Armor;
using FiveEData.Rules.Equipment.Shields;
using FiveEData.Rules.Equipment.Weapons;

namespace FiveEData.Rules.Catalog;

internal sealed class RulesetDefinitionSet
{
    public RulesetDefinitionSet(
        IReadOnlyList<WeaponDefinition> weapons,
        IReadOnlyList<SourceDocument> sourceDocuments,
        IReadOnlyList<AmmunitionDefinition> ammunition,
        IReadOnlyList<RuleDefinition> rules,
        IReadOnlyList<ArmorDefinition> armor,
        IReadOnlyList<ShieldDefinition> shields,
        ArmorUsageRules? armorUsage = null)
    {
        ArgumentNullException.ThrowIfNull(weapons);
        ArgumentNullException.ThrowIfNull(sourceDocuments);
        ArgumentNullException.ThrowIfNull(ammunition);
        ArgumentNullException.ThrowIfNull(rules);
        ArgumentNullException.ThrowIfNull(armor);
        ArgumentNullException.ThrowIfNull(shields);

        Weapons = weapons;
        SourceDocuments = sourceDocuments;
        Ammunition = ammunition;
        Rules = rules;
        Armor = armor;
        Shields = shields;
        ArmorUsage = armorUsage;
    }

    public IReadOnlyList<WeaponDefinition> Weapons { get; }
    public IReadOnlyList<SourceDocument> SourceDocuments { get; }
    public IReadOnlyList<AmmunitionDefinition> Ammunition { get; }
    public IReadOnlyList<RuleDefinition> Rules { get; }
    public IReadOnlyList<ArmorDefinition> Armor { get; }
    public IReadOnlyList<ShieldDefinition> Shields { get; }
    public ArmorUsageRules? ArmorUsage { get; }
}
