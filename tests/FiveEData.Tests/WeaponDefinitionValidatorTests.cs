using FiveEData.Rules.Equipment.Ammunition;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment.Weapons;

namespace FiveEData.Tests;

public sealed class WeaponDefinitionValidatorTests
{
    private static readonly SourceDocumentId PhbFirstPrinting =
        new("dnd5e2014.source.phb-first-printing");

    [Fact]
    public void Longsword_IsValid()
    {
        WeaponDefinition weapon = CreateWeapon(
            id: "dnd5e2014.weapon.longsword",
            name: "Longsword",
            proficiencyCategory: WeaponProficiencyCategory.Martial,
            usageCategory: WeaponUsageCategory.Melee,
            damage: DiceDamage(1, 8, DamageType.Slashing),
            properties: SetOf(
                WeaponProperty.Versatile),
            versatileDamage: new DiceExpression(1, 10));

        Assert.Empty(WeaponDefinitionValidator.Validate(weapon));
    }

    [Fact]
    public void Dagger_IsValid()
    {
        WeaponDefinition weapon = CreateWeapon(
            id: "dnd5e2014.weapon.dagger",
            name: "Dagger",
            proficiencyCategory: WeaponProficiencyCategory.Simple,
            usageCategory: WeaponUsageCategory.Melee,
            damage: DiceDamage(1, 4, DamageType.Piercing),
            properties: SetOf(
                WeaponProperty.Finesse,
                WeaponProperty.Light,
                WeaponProperty.Thrown),
            range: new WeaponRange(
                new Distance(20),
                new Distance(60)));

        Assert.Empty(WeaponDefinitionValidator.Validate(weapon));
    }

    [Fact]
    public void Longbow_IsValid()
    {
        WeaponDefinition weapon = CreateWeapon(
            id: "dnd5e2014.weapon.longbow",
            name: "Longbow",
            proficiencyCategory: WeaponProficiencyCategory.Martial,
            usageCategory: WeaponUsageCategory.Ranged,
            damage: DiceDamage(1, 8, DamageType.Piercing),
            properties: SetOf(
                WeaponProperty.Ammunition,
                WeaponProperty.Heavy,
                WeaponProperty.TwoHanded),
            range: new WeaponRange(
                new Distance(150),
                new Distance(600)),
            ammunitionTypeId:
                new AmmunitionTypeId("dnd5e2014.ammunition.arrow"));

        Assert.Empty(WeaponDefinitionValidator.Validate(weapon));
    }

    [Fact]
    public void Blowgun_IsValid_WithFixedDamage()
    {
        WeaponDefinition weapon = CreateWeapon(
            id: "dnd5e2014.weapon.blowgun",
            name: "Blowgun",
            proficiencyCategory: WeaponProficiencyCategory.Martial,
            usageCategory: WeaponUsageCategory.Ranged,
            damage: new WeaponDamage(
                dice: null,
                fixedAmount: 1,
                type: DamageType.Piercing),
            properties: SetOf(
                WeaponProperty.Ammunition,
                WeaponProperty.Loading),
            range: new WeaponRange(
                new Distance(25),
                new Distance(100)),
            ammunitionTypeId:
                new AmmunitionTypeId("dnd5e2014.ammunition.blowgun-needle"));

        Assert.Empty(WeaponDefinitionValidator.Validate(weapon));
    }

    [Fact]
    public void Lance_IsValid_WithSpecialRule()
    {
        WeaponDefinition weapon = CreateWeapon(
            id: "dnd5e2014.weapon.lance",
            name: "Lance",
            proficiencyCategory: WeaponProficiencyCategory.Martial,
            usageCategory: WeaponUsageCategory.Melee,
            damage: DiceDamage(1, 12, DamageType.Piercing),
            properties: SetOf(
                WeaponProperty.Reach,
                WeaponProperty.Special),
            specialRuleIds: SetOf(
                new RuleId("dnd5e2014.weapon-rule.lance")));

        Assert.Empty(WeaponDefinitionValidator.Validate(weapon));
    }

    [Fact]
    public void Net_IsValid_WithoutDamage()
    {
        WeaponDefinition weapon = CreateWeapon(
            id: "dnd5e2014.weapon.net",
            name: "Net",
            proficiencyCategory: WeaponProficiencyCategory.Martial,
            usageCategory: WeaponUsageCategory.Ranged,
            useDefaultDamage: false,
            properties: SetOf(
                WeaponProperty.Special,
                WeaponProperty.Thrown),
            range: new WeaponRange(
                new Distance(5),
                new Distance(15)),
            specialRuleIds: SetOf(
                new RuleId("dnd5e2014.weapon-rule.net")));

        Assert.Null(weapon.Damage);
        Assert.Empty(WeaponDefinitionValidator.Validate(weapon));
    }

    [Fact]
    public void ThrownWithoutRange_IsRejected()
    {
        WeaponDefinition weapon = CreateWeapon(
            properties: SetOf(
                WeaponProperty.Thrown));

        IReadOnlyList<string> errors =
            WeaponDefinitionValidator.Validate(weapon);

        Assert.Contains(
            "A weapon with the Thrown property must define a range.",
            errors);
    }

    [Fact]
    public void AmmunitionWithoutAmmunitionType_IsRejected()
    {
        WeaponDefinition weapon = CreateWeapon(
            usageCategory: WeaponUsageCategory.Ranged,
            properties: SetOf(
                WeaponProperty.Ammunition),
            range: new WeaponRange(
                new Distance(80),
                new Distance(320)));

        IReadOnlyList<string> errors =
            WeaponDefinitionValidator.Validate(weapon);

        Assert.Contains(
            "A weapon with the Ammunition property must define an ammunition type.",
            errors);
    }

    [Fact]
    public void VersatileWithoutVersatileDamage_IsRejected()
    {
        WeaponDefinition weapon = CreateWeapon(
            properties: SetOf(
                WeaponProperty.Versatile));

        IReadOnlyList<string> errors =
            WeaponDefinitionValidator.Validate(weapon);

        Assert.Contains(
            "A weapon with the Versatile property must define versatile damage.",
            errors);
    }

    [Fact]
    public void SpecialWithoutSpecialRule_IsRejected()
    {
        WeaponDefinition weapon = CreateWeapon(
            properties: SetOf(
                WeaponProperty.Special));

        IReadOnlyList<string> errors =
            WeaponDefinitionValidator.Validate(weapon);

        Assert.Contains(
            "A weapon with the Special property must reference at least one special rule.",
            errors);
    }

    [Fact]
    public void LightAndHeavyTogether_AreRejected()
    {
        WeaponDefinition weapon = CreateWeapon(
            properties: SetOf(
                WeaponProperty.Light,
                WeaponProperty.Heavy));

        IReadOnlyList<string> errors =
            WeaponDefinitionValidator.Validate(weapon);

        Assert.Contains(
            "A weapon cannot have both the Light and Heavy properties.",
            errors);
    }

    [Fact]
    public void TwoHandedAndVersatileTogether_AreRejected()
    {
        WeaponDefinition weapon = CreateWeapon(
            properties: SetOf(
                WeaponProperty.TwoHanded,
                WeaponProperty.Versatile),
            versatileDamage: new DiceExpression(1, 10));

        IReadOnlyList<string> errors =
            WeaponDefinitionValidator.Validate(weapon);

        Assert.Contains(
            "A weapon cannot have both the TwoHanded and Versatile properties.",
            errors);
    }

    private static WeaponDefinition CreateWeapon(
        string id = "dnd5e2014.weapon.test",
        string name = "Test Weapon",
        WeaponProficiencyCategory proficiencyCategory =
            WeaponProficiencyCategory.Simple,
        WeaponUsageCategory usageCategory =
            WeaponUsageCategory.Melee,
        WeaponDamage? damage = null,
        bool useDefaultDamage = true,
        IReadOnlySet<WeaponProperty>? properties = null,
        WeaponRange? range = null,
        DiceExpression? versatileDamage = null,
        AmmunitionTypeId? ammunitionTypeId = null,
        IReadOnlySet<RuleId>? specialRuleIds = null)
    {
        return new WeaponDefinition(
            new WeaponId(id),
            name,
            proficiencyCategory,
            usageCategory,
            cost: null,
            weight: null,
            damage: useDefaultDamage
                ? damage ?? DiceDamage(1, 4, DamageType.Bludgeoning)
                : damage,
            properties: properties ?? new HashSet<WeaponProperty>(),
            range,
            versatileDamage,
            ammunitionTypeId,
            specialRuleIds: specialRuleIds ?? new HashSet<RuleId>(),
            sources:
            [
                new SourceReference(
                    PhbFirstPrinting,
                    page: 149,
                    section: "Chapter 5: Equipment — Weapons")
            ]);
    }

    private static WeaponDamage DiceDamage(
        int count,
        int sides,
        DamageType type)
    {
        return new WeaponDamage(
            new DiceExpression(count, sides),
            fixedAmount: 0,
            type);
    }

    private static IReadOnlySet<T> SetOf<T>(params T[] values)
    {
        return new HashSet<T>(values);
    }
}
