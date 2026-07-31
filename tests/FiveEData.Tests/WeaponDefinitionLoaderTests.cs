using FiveEData.Rules.Equipment.Ammunition;
using FiveEData.Rules.Common;
using FiveEData.Rules.Equipment.Weapons;
using FiveEData.Rules.Equipment.Weapons.Serialization;

namespace FiveEData.Tests;

public sealed class WeaponDefinitionLoaderTests
{
    [Fact]
    public void LoadFromJson_MapsLongswordCorrectly()
    {
        const string json = """
        [
          {
            "id": "dnd5e2014.weapon.longsword",
            "name": "Longsword",
            "proficiencyCategory": "Martial",
            "usageCategory": "Melee",
            "cost": {
              "copperPieces": 1500
            },
            "weight": {
              "pounds": 3
            },
            "damage": {
              "dice": {
                "count": 1,
                "sides": 8
              },
              "fixedAmount": 0,
              "type": "Slashing"
            },
            "properties": [
              "Versatile"
            ],
            "range": null,
            "versatileDamage": {
              "count": 1,
              "sides": 10
            },
            "ammunitionTypeId": null,
            "specialRuleIds": [],
            "sources": [
              {
                "documentId": "dnd5e2014.source.phb-first-printing",
                "page": 149,
                "section": "Chapter 5: Equipment — Weapons"
              }
            ]
          }
        ]
        """;

        WeaponDefinition weapon =
            Assert.Single(WeaponDefinitionLoader.LoadFromJson(json));

        Assert.Equal(
            new WeaponId("dnd5e2014.weapon.longsword"),
            weapon.Id);

        Assert.Equal("Longsword", weapon.Name);
        Assert.Equal(
            WeaponProficiencyCategory.Martial,
            weapon.ProficiencyCategory);
        Assert.Equal(
            WeaponUsageCategory.Melee,
            weapon.UsageCategory);
        Assert.Equal(1500, weapon.Cost?.CopperPieces);
        Assert.Equal(3m, weapon.Weight?.Pounds);
        Assert.Equal(
            new DiceExpression(1, 8),
            weapon.Damage?.Dice);
        Assert.Equal(DamageType.Slashing, weapon.Damage?.Type);
        Assert.Contains(
            WeaponProperty.Versatile,
            weapon.Properties);
        Assert.Equal(
            new DiceExpression(1, 10),
            weapon.VersatileDamage);
        Assert.Null(weapon.Range);
        Assert.Null(weapon.AmmunitionTypeId);
        Assert.Empty(weapon.SpecialRuleIds);
        Assert.Single(weapon.Sources);
    }

    [Fact]
    public void LoadFromJson_PreservesDamageLessNet()
    {
        const string json = """
        [
          {
            "id": "dnd5e2014.weapon.net",
            "name": "Net",
            "proficiencyCategory": "Martial",
            "usageCategory": "Ranged",
            "cost": {
              "copperPieces": 100
            },
            "weight": {
              "pounds": 3
            },
            "damage": null,
            "properties": [
              "Special",
              "Thrown"
            ],
            "range": {
              "normalFeet": 5,
              "longFeet": 15
            },
            "versatileDamage": null,
            "ammunitionTypeId": null,
            "specialRuleIds": [
              "dnd5e2014.weapon-rule.net"
            ],
            "sources": [
              {
                "documentId": "dnd5e2014.source.phb-first-printing",
                "page": 149,
                "section": "Chapter 5: Equipment — Weapons"
              },
              {
                "documentId": "dnd5e2014.source.phb-first-printing",
                "page": 148,
                "section": "Chapter 5: Equipment — Special Weapons"
              }
            ]
          }
        ]
        """;

        WeaponDefinition weapon =
            Assert.Single(WeaponDefinitionLoader.LoadFromJson(json));

        Assert.Null(weapon.Damage);
        Assert.Equal(5, weapon.Range?.Normal.Feet);
        Assert.Equal(15, weapon.Range?.Long.Feet);
        Assert.Contains(
            WeaponProperty.Special,
            weapon.Properties);
        Assert.Contains(
            WeaponProperty.Thrown,
            weapon.Properties);
        Assert.Contains(
            new RuleId("dnd5e2014.weapon-rule.net"),
            weapon.SpecialRuleIds);
        Assert.Equal(2, weapon.Sources.Count);
    }

    [Fact]
    public void LoadFromJson_RejectsUnknownProperty()
    {
        const string json = """
        [
          {
            "id": "dnd5e2014.weapon.test",
            "name": "Test",
            "proficiencyCategory": "Simple",
            "usageCategory": "Melee",
            "properties": [],
            "specialRuleIds": [],
            "sources": [
              {
                "documentId": "dnd5e2014.source.phb-first-printing",
                "page": 149,
                "section": null
              }
            ],
            "madeUpField": true
          }
        ]
        """;

        Assert.Throws<InvalidDataException>(
            () => WeaponDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void LoadFromJson_RejectsInvalidDomainCombination()
    {
        const string json = """
        [
          {
            "id": "dnd5e2014.weapon.bad-versatile",
            "name": "Bad Versatile Weapon",
            "proficiencyCategory": "Simple",
            "usageCategory": "Melee",
            "damage": {
              "dice": {
                "count": 1,
                "sides": 8
              },
              "fixedAmount": 0,
              "type": "Slashing"
            },
            "properties": [
              "Versatile"
            ],
            "versatileDamage": null,
            "specialRuleIds": [],
            "sources": [
              {
                "documentId": "dnd5e2014.source.phb-first-printing",
                "page": 149,
                "section": null
              }
            ]
          }
        ]
        """;

        Assert.Throws<InvalidDataException>(
            () => WeaponDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void LoadFromJson_RejectsDuplicateWeaponIds()
    {
        const string json = """
        [
          {
            "id": "dnd5e2014.weapon.duplicate",
            "name": "First",
            "proficiencyCategory": "Simple",
            "usageCategory": "Melee",
            "damage": {
              "dice": {
                "count": 1,
                "sides": 4
              },
              "fixedAmount": 0,
              "type": "Bludgeoning"
            },
            "properties": [],
            "specialRuleIds": [],
            "sources": [
              {
                "documentId": "dnd5e2014.source.phb-first-printing",
                "page": 149,
                "section": null
              }
            ]
          },
          {
            "id": "dnd5e2014.weapon.duplicate",
            "name": "Second",
            "proficiencyCategory": "Simple",
            "usageCategory": "Melee",
            "damage": {
              "dice": {
                "count": 1,
                "sides": 6
              },
              "fixedAmount": 0,
              "type": "Bludgeoning"
            },
            "properties": [],
            "specialRuleIds": [],
            "sources": [
              {
                "documentId": "dnd5e2014.source.phb-first-printing",
                "page": 149,
                "section": null
              }
            ]
          }
        ]
        """;

        Assert.Throws<InvalidDataException>(
            () => WeaponDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void LoadFromJson_NullArrayElement_IsRejectedAsDataError()
    {
        InvalidDataException exception = Assert.Throws<InvalidDataException>(
            () => WeaponDefinitionLoader.LoadFromJson("[null]"));

        Assert.Contains("index 0", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void LoadFromJson_RejectsNumericEnumValues()
    {
        const string json = """
        [
          {
            "id": "dnd5e2014.weapon.test",
            "name": "Test",
            "proficiencyCategory": 0,
            "usageCategory": "Melee",
            "properties": [],
            "specialRuleIds": [],
            "sources": [
              {
                "documentId": "dnd5e2014.source.phb-first-printing",
                "page": 149,
                "section": null
              }
            ]
          }
        ]
        """;

        Assert.Throws<InvalidDataException>(
            () => WeaponDefinitionLoader.LoadFromJson(json));
    }
}
