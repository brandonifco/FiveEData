using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.DamageTypes;
using FiveEData.Rules.Equipment.Ammunition;
using FiveEData.Rules.Equipment.Weapons;

namespace FiveEData.Tests;

public sealed class DomainImmutabilityTests
{
    [Fact]
    public void WeaponDefinition_DefensivelySnapshotsCollections()
    {
        var properties = new HashSet<WeaponProperty>
        {
            WeaponProperty.Light
        };

        var specialRuleIds = new HashSet<RuleId>
        {
            new("dnd5e2014.weapon-rule.original")
        };

        var sources = new List<SourceReference>
        {
            new(
                new SourceDocumentId(
                    "dnd5e2014.source.phb-first-printing"),
                page: 149)
        };

        WeaponDefinition weapon = new(
            new WeaponId("dnd5e2014.weapon.test"),
            "Test",
            WeaponProficiencyCategory.Simple,
            WeaponUsageCategory.Melee,
            cost: null,
            weight: null,
            damage: new WeaponDamage(
                new DiceExpression(1, 4),
                fixedAmount: 0,
                new DamageTypeId("dnd5e2014.damage-type.piercing")),
            properties,
            range: null,
            versatileDamage: null,
            ammunitionTypeId: null,
            specialRuleIds,
            sources);

        properties.Add(WeaponProperty.Heavy);
        specialRuleIds.Add(
            new RuleId("dnd5e2014.weapon-rule.added-later"));
        sources.Add(
            new SourceReference(
                new SourceDocumentId(
                    "dnd5e2014.source.other"),
                page: 1));

        Assert.Single(weapon.Properties);
        Assert.Contains(WeaponProperty.Light, weapon.Properties);
        Assert.DoesNotContain(WeaponProperty.Heavy, weapon.Properties);

        Assert.Single(weapon.SpecialRuleIds);
        Assert.Contains(
            new RuleId("dnd5e2014.weapon-rule.original"),
            weapon.SpecialRuleIds);

        Assert.Single(weapon.Sources);
        Assert.Equal(
            new SourceDocumentId(
                "dnd5e2014.source.phb-first-printing"),
            weapon.Sources[0].DocumentId);
    }

    [Fact]
    public void AmmunitionDefinition_DefensivelySnapshotsSources()
    {
        var sources = new List<SourceReference>
        {
            new(
                new SourceDocumentId(
                    "dnd5e2014.source.phb-first-printing"),
                page: 150)
        };

        AmmunitionDefinition ammunition = new(
            new AmmunitionTypeId("dnd5e2014.ammunition.test"),
            "Test ammunition",
            bundleQuantity: 20,
            new Money(100),
            new Weight(1m),
            sources);

        sources.Add(
            new SourceReference(
                new SourceDocumentId(
                    "dnd5e2014.source.other"),
                page: 1));

        Assert.Single(ammunition.Sources);
        Assert.Equal(
            new SourceDocumentId(
                "dnd5e2014.source.phb-first-printing"),
            ammunition.Sources[0].DocumentId);
    }

    [Fact]
    public void WeaponDefinition_ExposesNoPublicSetters()
    {
        Type type = typeof(WeaponDefinition);

        Assert.All(
            type.GetProperties(),
            property => Assert.Null(property.SetMethod));
    }

    [Fact]
    public void AmmunitionDefinition_ExposesNoPublicSetters()
    {
        Type type = typeof(AmmunitionDefinition);

        Assert.All(
            type.GetProperties(),
            property => Assert.Null(property.SetMethod));
    }
}
