using FiveEData.Rules.Classes.FightingStyles;
using FiveEData.Rules.Classes.FightingStyles.Serialization;

namespace FiveEData.Tests;

public sealed class FightingStyleDefinitionLoaderTests
{
    [Fact]
    public void ValidDefinition_LoadsStrictly()
    {
        FightingStyleDefinition definition = Assert.Single(
            FightingStyleDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.fighting-style.test",
                    "name": "Test",
                    "availableToClassIds": ["extension.class.test"],
                    "rollBonus": {
                      "target": "AttackRoll",
                      "amount": 2,
                      "weaponRequirement": "RangedWeapon"
                    },
                    "armorClassBonus": null,
                    "damageDieReroll": null,
                    "reaction": null,
                    "grantsOffHandAbilityModifierDamage": false,
                    "sources": [
                      {
                        "documentId": "extension.source.test",
                        "page": 1,
                        "section": "Test section"
                      }
                    ]
                  }
                ]
                """));

        Assert.Equal(
            "extension.fighting-style.test",
            definition.Id.Value);
        Assert.Equal("Test", definition.Name);
        Assert.Equal(
            "extension.class.test",
            Assert.Single(definition.AvailableToClassIds).Value);
        Assert.NotNull(definition.RollBonus);
        Assert.Equal(
            FightingStyleRollTarget.AttackRoll,
            definition.RollBonus!.Value.Target);
        Assert.Equal(2, definition.RollBonus.Value.Amount);
        Assert.Equal(
            FightingStyleWeaponRequirement.RangedWeapon,
            definition.RollBonus.Value.WeaponRequirement);
        Assert.Null(definition.ArmorClassBonus);
        Assert.Null(definition.DamageDieReroll);
        Assert.Null(definition.Reaction);
        Assert.False(definition.GrantsOffHandAbilityModifierDamage);
        Assert.Single(definition.Sources);
    }

    [Fact]
    public void ValidDefinition_LoadsArmorClassBonusMechanism()
    {
        FightingStyleDefinition definition = Assert.Single(
            FightingStyleDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.fighting-style.test",
                    "name": "Test",
                    "availableToClassIds": ["extension.class.test"],
                    "rollBonus": null,
                    "armorClassBonus": 1,
                    "damageDieReroll": null,
                    "reaction": null,
                    "grantsOffHandAbilityModifierDamage": false,
                    "sources": [
                      {
                        "documentId": "extension.source.test",
                        "page": 1,
                      "section": "Test section"
                      }
                    ]
                  }
                ]
                """));

        Assert.Equal(1, definition.ArmorClassBonus);
    }

    [Fact]
    public void ValidDefinition_LoadsDamageDieRerollMechanism()
    {
        FightingStyleDefinition definition = Assert.Single(
            FightingStyleDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.fighting-style.test",
                    "name": "Test",
                    "availableToClassIds": ["extension.class.test"],
                    "rollBonus": null,
                    "armorClassBonus": null,
                    "damageDieReroll": {
                      "rerollAtOrBelowValue": 2,
                      "weaponRequirement": "MeleeWeaponWithTwoHandedOrVersatileProperty"
                    },
                    "reaction": null,
                    "grantsOffHandAbilityModifierDamage": false,
                    "sources": [
                      {
                        "documentId": "extension.source.test",
                        "page": 1,
                      "section": "Test section"
                      }
                    ]
                  }
                ]
                """));

        Assert.NotNull(definition.DamageDieReroll);
        Assert.Equal(
            2,
            definition.DamageDieReroll!.Value.RerollAtOrBelowValue);
    }

    [Fact]
    public void ValidDefinition_LoadsReactionMechanism()
    {
        FightingStyleDefinition definition = Assert.Single(
            FightingStyleDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.fighting-style.test",
                    "name": "Test",
                    "availableToClassIds": ["extension.class.test"],
                    "rollBonus": null,
                    "armorClassBonus": null,
                    "damageDieReroll": null,
                    "reaction": {
                      "rangeFeet": 5,
                      "requiresShield": true
                    },
                    "grantsOffHandAbilityModifierDamage": false,
                    "sources": [
                      {
                        "documentId": "extension.source.test",
                        "page": 1,
                      "section": "Test section"
                      }
                    ]
                  }
                ]
                """));

        Assert.NotNull(definition.Reaction);
        Assert.Equal(5, definition.Reaction!.Value.Range.Feet);
        Assert.True(definition.Reaction.Value.RequiresShield);
    }

    [Fact]
    public void NullRootArray_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => FightingStyleDefinitionLoader.LoadFromJson("null"));
    }

    [Fact]
    public void NullArrayElement_IsRejectedAsDataError()
    {
        InvalidDataException exception =
            Assert.Throws<InvalidDataException>(
                () => FightingStyleDefinitionLoader.LoadFromJson("[null]"));

        Assert.Contains(
            "index 0",
            exception.Message,
            StringComparison.Ordinal);
    }

    [Fact]
    public void UnknownProperty_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => FightingStyleDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.fighting-style.test",
                    "name": "Test",
                    "availableToClassIds": [],
                    "rollBonus": null,
                    "armorClassBonus": null,
                    "damageDieReroll": null,
                    "reaction": null,
                    "grantsOffHandAbilityModifierDamage": false,
                    "sources": [],
                    "unexpected": true
                  }
                ]
                """));
    }

    [Fact]
    public void DuplicateJsonProperty_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => FightingStyleDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.fighting-style.test",
                    "name": "Test",
                    "name": "Other",
                    "availableToClassIds": [],
                    "rollBonus": null,
                    "armorClassBonus": null,
                    "damageDieReroll": null,
                    "reaction": null,
                    "grantsOffHandAbilityModifierDamage": false,
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void MissingRequiredSourcesMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => FightingStyleDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.fighting-style.test",
                    "name": "Test",
                    "availableToClassIds": [],
                    "rollBonus": null,
                    "armorClassBonus": null,
                    "damageDieReroll": null,
                    "reaction": null,
                    "grantsOffHandAbilityModifierDamage": false
                  }
                ]
                """));
    }

    [Fact]
    public void NullRequiredIdMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => FightingStyleDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": null,
                    "name": "Test",
                    "availableToClassIds": [],
                    "rollBonus": null,
                    "armorClassBonus": null,
                    "damageDieReroll": null,
                    "reaction": null,
                    "grantsOffHandAbilityModifierDamage": false,
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void NullRequiredAvailableToClassIdsMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => FightingStyleDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.fighting-style.test",
                    "name": "Test",
                    "availableToClassIds": null,
                    "rollBonus": null,
                    "armorClassBonus": null,
                    "damageDieReroll": null,
                    "reaction": null,
                    "grantsOffHandAbilityModifierDamage": false,
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void DuplicateIds_AreRejected()
    {
        const string one =
            """
            {
              "id": "extension.fighting-style.test",
              "name": "Test",
              "availableToClassIds": ["extension.class.test"],
              "rollBonus": null,
              "armorClassBonus": 1,
              "damageDieReroll": null,
              "reaction": null,
              "grantsOffHandAbilityModifierDamage": false,
              "sources": [
                {
                  "documentId": "extension.source.test",
                  "page": 1,
                "section": "Test section"
                }
              ]
            }
            """;

        string json = $"[{one},{one}]";

        Assert.Throws<InvalidDataException>(
            () => FightingStyleDefinitionLoader.LoadFromJson(json));
    }
}
