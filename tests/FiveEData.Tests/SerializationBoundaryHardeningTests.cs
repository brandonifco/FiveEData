using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Equipment.Ammunition.Serialization;
using FiveEData.Rules.Equipment.Weapons.Serialization;

namespace FiveEData.Tests;

public sealed class SerializationBoundaryHardeningTests
{
    [Fact]
    public void WeaponJson_MissingProficiencyCategory_IsRejected()
    {
        const string json = """
        [
          {
            "id": "dnd5e2014.weapon.test",
            "name": "Test",
            "usageCategory": "Melee",
            "cost": null,
            "weight": null,
            "damage": null,
            "properties": [],
            "range": null,
            "versatileDamage": null,
            "ammunitionTypeId": null,
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
    public void WeaponJson_MissingProperties_IsRejected()
    {
        const string json = """
        [
          {
            "id": "dnd5e2014.weapon.test",
            "name": "Test",
            "proficiencyCategory": "Simple",
            "usageCategory": "Melee",
            "cost": null,
            "weight": null,
            "damage": null,
            "range": null,
            "versatileDamage": null,
            "ammunitionTypeId": null,
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
    public void WeaponJson_ExplicitNullProperties_IsRejected()
    {
        const string json = """
        [
          {
            "id": "dnd5e2014.weapon.test",
            "name": "Test",
            "proficiencyCategory": "Simple",
            "usageCategory": "Melee",
            "cost": null,
            "weight": null,
            "damage": null,
            "properties": null,
            "range": null,
            "versatileDamage": null,
            "ammunitionTypeId": null,
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
    public void WeaponDamageJson_MissingDamageType_IsRejected()
    {
        const string json = """
        [
          {
            "id": "dnd5e2014.weapon.test",
            "name": "Test",
            "proficiencyCategory": "Simple",
            "usageCategory": "Melee",
            "cost": null,
            "weight": null,
            "damage": {
              "dice": {
                "count": 1,
                "sides": 4
              },
              "fixedAmount": 0
            },
            "properties": [],
            "range": null,
            "versatileDamage": null,
            "ammunitionTypeId": null,
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
    public void AmmunitionJson_MissingBundleQuantity_IsRejected()
    {
        const string json = """
        [
          {
            "id": "dnd5e2014.ammunition.test",
            "name": "Test ammunition",
            "cost": {
              "copperPieces": 100
            },
            "weight": {
              "pounds": 1
            },
            "sources": [
              {
                "documentId": "dnd5e2014.source.phb-first-printing",
                "page": 150,
                "section": null
              }
            ]
          }
        ]
        """;

        Assert.Throws<InvalidDataException>(
            () => AmmunitionDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void SourceDocumentJson_MissingTitle_IsRejected()
    {
        const string json = """
        [
          {
            "id": "dnd5e2014.source.test",
            "edition": null,
            "printing": null,
            "publicationDate": null,
            "isbn": null
          }
        ]
        """;

        Assert.Throws<InvalidDataException>(
            () => SourceDocumentLoader.LoadFromJson(json));
    }

    [Fact]
    public void SourceReferenceJson_MissingPage_IsRejected()
    {
        const string json = """
        [
          {
            "id": "dnd5e2014.weapon.test",
            "name": "Test",
            "proficiencyCategory": "Simple",
            "usageCategory": "Melee",
            "cost": null,
            "weight": null,
            "damage": null,
            "properties": [],
            "range": null,
            "versatileDamage": null,
            "ammunitionTypeId": null,
            "specialRuleIds": [],
            "sources": [
              {
                "documentId": "dnd5e2014.source.phb-first-printing",
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
    public void UnknownJsonMember_IsRejectedBySharedPolicy()
    {
        const string json = """
        [
          {
            "id": "dnd5e2014.source.test",
            "title": "Test",
            "edition": null,
            "printing": null,
            "publicationDate": null,
            "isbn": null,
            "unexpected": true
          }
        ]
        """;

        Assert.Throws<InvalidDataException>(
            () => SourceDocumentLoader.LoadFromJson(json));
    }
}
