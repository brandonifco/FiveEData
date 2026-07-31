using System.Threading;
using FiveEData.Rules.Catalog;
using FiveEData.Rules.Equipment.Armor;

namespace FiveEData;

public sealed class Dnd5e2014Ruleset
{
    private static readonly Lazy<Dnd5e2014Ruleset> LazyInstance =
        new(
            Dnd5e2014RulesetLoader.Load,
            LazyThreadSafetyMode.ExecutionAndPublication);

    internal Dnd5e2014Ruleset(
        WeaponCatalog weapons,
        AmmunitionCatalog ammunition,
        ArmorCatalog armor,
        ShieldCatalog shields,
        AdventuringGearCatalog adventuringGear,
        ArmorUsageRules armorUsage,
        SourceDocumentCatalog sources,
        RuleCatalog rules)
    {
        ArgumentNullException.ThrowIfNull(weapons);
        ArgumentNullException.ThrowIfNull(ammunition);
        ArgumentNullException.ThrowIfNull(armor);
        ArgumentNullException.ThrowIfNull(shields);
        ArgumentNullException.ThrowIfNull(adventuringGear);
        ArgumentNullException.ThrowIfNull(armorUsage);
        ArgumentNullException.ThrowIfNull(sources);
        ArgumentNullException.ThrowIfNull(rules);

        Weapons = weapons;
        Ammunition = ammunition;
        Armor = armor;
        Shields = shields;
        AdventuringGear = adventuringGear;
        ArmorUsage = armorUsage;
        Sources = sources;
        Rules = rules;
    }

    public static Dnd5e2014Ruleset Instance => LazyInstance.Value;

    public WeaponCatalog Weapons { get; }
    public AmmunitionCatalog Ammunition { get; }
    public ArmorCatalog Armor { get; }
    public ShieldCatalog Shields { get; }
    public AdventuringGearCatalog AdventuringGear { get; }
    public ArmorUsageRules ArmorUsage { get; }
    public SourceDocumentCatalog Sources { get; }
    public RuleCatalog Rules { get; }
}
