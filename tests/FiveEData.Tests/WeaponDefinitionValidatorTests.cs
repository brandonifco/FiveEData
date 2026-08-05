using FiveEData.Rules.Equipment.Ammunition;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.DamageTypes;
using FiveEData.Rules.Equipment.Weapons;

namespace FiveEData.Tests;

public sealed class WeaponDefinitionValidatorTests
{
    private static readonly SourceDocumentId PhbFirstPrinting =
        new("dnd5e2014.source.phb-first-printing");

    private static readonly DamageTypeId Bludgeoning =
        new("dnd5e2014.damage-type.bludgeoning");
    private static readonly DamageTypeId Piercing =
        new("dnd5e2014.damage-type.piercing");
    private static readonly DamageTypeId Slashing =
        new("dnd5e2014.damage-type.slashing");

    [Fact]
    public void Longsword_IsValid()
    {
        WeaponDefinition weapon = CreateWeapon(
            id: "dnd5e2014.weapon.longsword",
            name: "Longsword",
            proficiencyCategory: WeaponProficiencyCategory.Martial,
            usageCategory: WeaponUsageCategory.Melee,
            damage: DiceDamage(1, 8, Slashing),
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
            damage: DiceDamage(1, 4, Piercing),
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
            damage: DiceDamage(1, 8, Piercing),
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
                damageTypeId: Piercing),
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
            damage: DiceDamage(1, 12, Piercing),
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
    public void DefaultWeaponId_IsRejected()
    {
        WeaponDefinition weapon = CreateWeapon(
            typedId: default(WeaponId));

        IReadOnlyList<string> errors =
            WeaponDefinitionValidator.Validate(weapon);

        Assert.Contains(
            "Weapon ID must not be empty.",
            errors);
    }

    [Fact]
    public void UndefinedProficiencyCategory_IsRejected()
    {
        WeaponDefinition weapon = CreateWeapon(
            proficiencyCategory:
                (WeaponProficiencyCategory)999);

        IReadOnlyList<string> errors =
            WeaponDefinitionValidator.Validate(weapon);

        Assert.Contains(
            "Weapon proficiency category must be defined.",
            errors);
    }

    [Fact]
    public void UndefinedUsageCategory_IsRejected()
    {
        WeaponDefinition weapon = CreateWeapon(
            usageCategory:
                (WeaponUsageCategory)999);

        IReadOnlyList<string> errors =
            WeaponDefinitionValidator.Validate(weapon);

        Assert.Contains(
            "Weapon usage category must be defined.",
            errors);
    }

    [Fact]
    public void UndefinedWeaponProperty_IsRejected()
    {
        WeaponDefinition weapon = CreateWeapon(
            properties: SetOf((WeaponProperty)999));

        IReadOnlyList<string> errors =
            WeaponDefinitionValidator.Validate(weapon);

        Assert.Contains(
            errors,
            error => error.Contains(
                "Weapon property value '999' must be defined.",
                StringComparison.Ordinal));
    }

    [Fact]
    public void DefaultAmmunitionTypeId_WhenPresent_IsRejected()
    {
        WeaponDefinition weapon = CreateWeapon(
            ammunitionTypeId: default(AmmunitionTypeId));

        IReadOnlyList<string> errors =
            WeaponDefinitionValidator.Validate(weapon);

        Assert.Contains(
            "Weapon ammunition type ID must not be empty " +
            "when specified.",
            errors);
    }

    [Fact]
    public void DefaultSpecialRuleId_IsRejected()
    {
        WeaponDefinition weapon = CreateWeapon(
            specialRuleIds: SetOf(default(RuleId)));

        IReadOnlyList<string> errors =
            WeaponDefinitionValidator.Validate(weapon);

        Assert.Contains(
            "Weapon special rule ID must not be empty.",
            errors);
    }

    [Fact]
    public void MultipleLocalInvariantErrors_AreReturnedTogether()
    {
        WeaponDefinition weapon = CreateWeapon(
            typedId: default(WeaponId),
            proficiencyCategory:
                (WeaponProficiencyCategory)999,
            usageCategory:
                (WeaponUsageCategory)999,
            properties: SetOf((WeaponProperty)999),
            ammunitionTypeId: default(AmmunitionTypeId),
            specialRuleIds: SetOf(default(RuleId)));

        IReadOnlyList<string> errors =
            WeaponDefinitionValidator.Validate(weapon);

        Assert.Contains(
            "Weapon ID must not be empty.",
            errors);
        Assert.Contains(
            "Weapon proficiency category must be defined.",
            errors);
        Assert.Contains(
            "Weapon usage category must be defined.",
            errors);
        Assert.Contains(
            errors,
            error => error.Contains(
                "Weapon property value '999' must be defined.",
                StringComparison.Ordinal));
        Assert.Contains(
            "Weapon ammunition type ID must not be empty " +
            "when specified.",
            errors);
        Assert.Contains(
            "Weapon special rule ID must not be empty.",
            errors);
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
        WeaponId? typedId = null,
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
            typedId ?? new WeaponId(id),
            name,
            proficiencyCategory,
            usageCategory,
            cost: null,
            weight: null,
            damage: useDefaultDamage
                ? damage ?? DiceDamage(1, 4, Bludgeoning)
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
        DamageTypeId type)
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
