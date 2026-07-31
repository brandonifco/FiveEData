using System.Reflection;
using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment.Ammunition;
using FiveEData.Rules.Equipment.Armor;
using FiveEData.Rules.Equipment.Shields;
using FiveEData.Rules.Equipment.Weapons;

namespace FiveEData.Tests;

public sealed class DomainApiSemanticsTests
{
    [Fact]
    public void IdentityBearingDefinitions_HaveNoPublicConstructors()
    {
        AssertNoPublicConstructors(typeof(WeaponDefinition));
        AssertNoPublicConstructors(typeof(AmmunitionDefinition));
        AssertNoPublicConstructors(typeof(ArmorDefinition));
        AssertNoPublicConstructors(typeof(ShieldDefinition));
        AssertNoPublicConstructors(typeof(ArmorUsageRules));
        AssertNoPublicConstructors(typeof(RuleDefinition));
        AssertNoPublicConstructors(typeof(SourceDocument));
    }

    [Fact]
    public void Catalogs_HaveNoPublicConstructors()
    {
        AssertNoPublicConstructors(typeof(WeaponCatalog));
        AssertNoPublicConstructors(typeof(AmmunitionCatalog));
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
        Assert.Same(first.Armor, second.Armor);
        Assert.Same(first.Shields, second.Shields);
        Assert.Same(first.ArmorUsage, second.ArmorUsage);
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
                DamageType.Piercing),
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
                DamageType.Bludgeoning),
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
