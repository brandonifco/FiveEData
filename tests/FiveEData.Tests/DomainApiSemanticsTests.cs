using System.Reflection;
using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.DamageTypes;
using FiveEData.Rules.Creatures.Languages;
using FiveEData.Rules.Creatures.Sizes;
using FiveEData.Rules.Creatures.Skills;
using FiveEData.Rules.Equipment.Ammunition;
using FiveEData.Rules.Equipment.AdventuringGear;
using FiveEData.Rules.Equipment.Armor;
using FiveEData.Rules.Equipment.Mounts;
using FiveEData.Rules.Equipment.MountSupport;
using FiveEData.Rules.Equipment.MountsAndVehicles;
using FiveEData.Rules.Equipment.TradeGoods;
using FiveEData.Rules.Equipment.Vehicles;
using FiveEData.Rules.Equipment.Shields;
using FiveEData.Rules.Equipment.Weapons;
using FiveEData.Rules.Equipment.Tools;

namespace FiveEData.Tests;

public sealed class DomainApiSemanticsTests
{
    [Fact]
    public void IdentityBearingDefinitions_HaveNoPublicConstructors()
    {
        AssertNoPublicConstructors(typeof(AbilityDefinition));
        AssertNoPublicConstructors(typeof(SkillDefinition));
        AssertNoPublicConstructors(typeof(LanguageDefinition));
        AssertNoPublicConstructors(
            typeof(CreatureSizeDefinition));
        AssertNoPublicConstructors(typeof(WeaponDefinition));
        AssertNoPublicConstructors(typeof(AmmunitionDefinition));
        AssertNoPublicConstructors(typeof(AdventuringGearDefinition));
        AssertNoPublicConstructors(typeof(ContainerCapacityDefinition));
        AssertNoPublicConstructors(typeof(ToolDefinition));
        AssertNoPublicConstructors(typeof(ToolFamilyDefinition));
        AssertNoPublicConstructors(typeof(MountDefinition));
        AssertNoPublicConstructors(typeof(MountSupportDefinition));
        AssertNoPublicConstructors(typeof(MountVehicleRules));
        AssertNoPublicConstructors(typeof(TradeGoodDefinition));
        AssertNoPublicConstructors(typeof(VehicleDefinition));
        AssertNoPublicConstructors(typeof(ArmorDefinition));
        AssertNoPublicConstructors(typeof(ShieldDefinition));
        AssertNoPublicConstructors(typeof(ArmorUsageRules));
        AssertNoPublicConstructors(typeof(RuleDefinition));
        AssertNoPublicConstructors(typeof(SourceDocument));
    }

    [Fact]
    public void Catalogs_HaveNoPublicConstructors()
    {
        AssertNoPublicConstructors(typeof(AbilityCatalog));
        AssertNoPublicConstructors(typeof(SkillCatalog));
        AssertNoPublicConstructors(typeof(LanguageCatalog));
        AssertNoPublicConstructors(typeof(CreatureSizeCatalog));
        AssertNoPublicConstructors(
            typeof(CreatureVocabularyCatalogs));
        AssertNoPublicConstructors(typeof(WeaponCatalog));
        AssertNoPublicConstructors(typeof(AmmunitionCatalog));
        AssertNoPublicConstructors(typeof(AdventuringGearCatalog));
        AssertNoPublicConstructors(typeof(ContainerCapacityCatalog));
        AssertNoPublicConstructors(typeof(ToolCatalog));
        AssertNoPublicConstructors(typeof(ToolFamilyCatalog));
        AssertNoPublicConstructors(typeof(MountCatalog));
        AssertNoPublicConstructors(typeof(MountSupportCatalog));
        AssertNoPublicConstructors(typeof(TradeGoodCatalog));
        AssertNoPublicConstructors(typeof(VehicleCatalog));
        AssertNoPublicConstructors(typeof(ArmorCatalog));
        AssertNoPublicConstructors(typeof(ShieldCatalog));
        AssertNoPublicConstructors(typeof(RuleCatalog));
        AssertNoPublicConstructors(typeof(SourceDocumentCatalog));
    }

    [Fact]
    public void OfficialRuleset_IsCachedAndReferenceStable()
    {
        Dnd5e2014Ruleset first = Dnd5e2014Ruleset.Instance;
        Dnd5e2014Ruleset second = Dnd5e2014Ruleset.Instance;

        Assert.Same(first, second);
        Assert.Same(first.Weapons, second.Weapons);
        Assert.Same(first.Ammunition, second.Ammunition);
        Assert.Same(first.AdventuringGear, second.AdventuringGear);
        Assert.Same(first.ContainerCapacities, second.ContainerCapacities);
        Assert.Same(first.ToolFamilies, second.ToolFamilies);
        Assert.Same(first.Tools, second.Tools);
        Assert.Same(first.Mounts, second.Mounts);
        Assert.Same(first.Vehicles, second.Vehicles);
        Assert.Same(first.MountSupport, second.MountSupport);
        Assert.Same(first.MountVehicleRules, second.MountVehicleRules);
        Assert.Same(first.TradeGoods, second.TradeGoods);
        Assert.Same(first.Armor, second.Armor);
        Assert.Same(first.Shields, second.Shields);
        Assert.Same(first.ArmorUsage, second.ArmorUsage);
        Assert.Same(
            first.CreatureVocabulary,
            second.CreatureVocabulary);
        Assert.Same(
            first.CreatureVocabulary.Abilities,
            second.CreatureVocabulary.Abilities);
        Assert.Same(
            first.CreatureVocabulary.Skills,
            second.CreatureVocabulary.Skills);
        Assert.Same(
            first.CreatureVocabulary.Languages,
            second.CreatureVocabulary.Languages);
        Assert.Same(
            first.CreatureVocabulary.Sizes,
            second.CreatureVocabulary.Sizes);
        Assert.Same(first.Rules, second.Rules);
        Assert.Same(first.Sources, second.Sources);
    }

    [Fact]
    public void IdentityBearingDefinitions_UseReferenceIdentity()
    {
        WeaponDefinition first = CreateTestWeapon();
        WeaponDefinition second = CreateTestWeapon();

        Assert.NotSame(first, second);
        Assert.False(first.Equals(second));
    }

    [Fact]
    public void InvalidDefinition_CannotEnterCatalog()
    {
        WeaponDefinition invalid = new(
            new WeaponId("dnd5e2014.weapon.invalid"),
            "Invalid",
            WeaponProficiencyCategory.Simple,
            WeaponUsageCategory.Ranged,
            cost: null,
            weight: null,
            damage: new WeaponDamage(
                new DiceExpression(1, 4),
                fixedAmount: 0,
                new DamageTypeId("dnd5e2014.damage-type.piercing")),
            properties:
            [
                WeaponProperty.Ammunition
            ],
            range: null,
            versatileDamage: null,
            ammunitionTypeId: null,
            specialRuleIds: [],
            sources:
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 149)
            ]);

        Assert.Throws<InvalidOperationException>(
            () => new WeaponCatalog([invalid]));
    }

    private static void AssertNoPublicConstructors(Type type)
    {
        ConstructorInfo[] constructors =
            type.GetConstructors(
                BindingFlags.Public |
                BindingFlags.Instance);

        Assert.Empty(constructors);
    }

    private static WeaponDefinition CreateTestWeapon()
    {
        return new WeaponDefinition(
            new WeaponId("dnd5e2014.weapon.identity-test"),
            "Identity Test",
            WeaponProficiencyCategory.Simple,
            WeaponUsageCategory.Melee,
            cost: null,
            weight: null,
            damage: new WeaponDamage(
                new DiceExpression(1, 4),
                fixedAmount: 0,
                new DamageTypeId("dnd5e2014.damage-type.bludgeoning")),
            properties: [],
            range: null,
            versatileDamage: null,
            ammunitionTypeId: null,
            specialRuleIds: [],
            sources:
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 149)
            ]);
    }
}
