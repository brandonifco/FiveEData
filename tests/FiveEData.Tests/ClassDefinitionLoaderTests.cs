using FiveEData.Rules.Classes;
using FiveEData.Rules.Classes.Serialization;
using FiveEData.Rules.Equipment.Armor;
using FiveEData.Rules.Equipment.Weapons;

namespace FiveEData.Tests;

public sealed class ClassDefinitionLoaderTests
{
    private const string ValidClass =
        """
        {
          "id": "extension.class.test",
          "name": "Test",
          "hitDieSides": 10,
          "primaryAbilityIds": ["dnd5e2014.ability.strength"],
          "requiresAllPrimaryAbilities": false,
          "savingThrowProficiencyIds": [
            "dnd5e2014.ability.strength",
            "dnd5e2014.ability.constitution"
          ],
          "armorProficiencyCategories": ["Light", "Medium", "Heavy"],
          "proficientWithShields": true,
          "weaponProficiencyCategories": ["Simple", "Martial"],
          "weaponProficiencyIds": [],
          "skillChoiceCount": 2,
          "skillChoiceOptionIds": [
            "dnd5e2014.skill.athletics",
            "dnd5e2014.skill.perception"
          ],
          "levelFeatures": [
            {
              "level": 1,
              "featureRuleId": "dnd5e2014.class-rule.test"
            }
          ],
          "spellSlotProgressionId": null,
          "spellcastingAbilityId": null,
          "extraAttackProgressionId": null,
          "sources": [
            {
              "documentId": "extension.source.test",
              "page": 1,
              "section": "Test section"
            }
          ]
        }
        """;

    [Fact]
    public void ValidDefinition_LoadsStrictly()
    {
        ClassDefinition @class = Assert.Single(
            ClassDefinitionLoader.LoadFromJson($"[{ValidClass}]"));

        Assert.Equal("extension.class.test", @class.Id.Value);
        Assert.Equal("Test", @class.Name);
        Assert.Equal(1, @class.HitDie.Count);
        Assert.Equal(10, @class.HitDie.Sides);
        Assert.Equal(
            "dnd5e2014.ability.strength",
            Assert.Single(@class.PrimaryAbilityIds).Value);
        Assert.False(@class.RequiresAllPrimaryAbilities);
        Assert.Equal(2, @class.SavingThrowProficiencyIds.Count);
        Assert.Equal(
            [ArmorCategory.Light, ArmorCategory.Medium, ArmorCategory.Heavy],
            @class.ArmorProficiencyCategories);
        Assert.True(@class.ProficientWithShields);
        Assert.Equal(
            [WeaponProficiencyCategory.Simple, WeaponProficiencyCategory.Martial],
            @class.WeaponProficiencyCategories);
        Assert.Empty(@class.WeaponProficiencyIds);
        Assert.Equal(2, @class.SkillChoiceCount);
        Assert.Equal(2, @class.SkillChoiceOptionIds.Count);

        ClassLevelFeature feature = Assert.Single(@class.LevelFeatures);
        Assert.Equal(1, feature.Level);
        Assert.Equal(
            "dnd5e2014.class-rule.test",
            feature.FeatureRuleId.Value);

        Assert.Null(@class.SpellSlotProgressionId);
        Assert.Null(@class.SpellcastingAbilityId);
        Assert.Null(@class.ExtraAttackProgressionId);
        Assert.Single(@class.Sources);
    }

    [Fact]
    public void ValidDefinition_LoadsSpellcastingFields()
    {
        ClassDefinition @class = Assert.Single(
            ClassDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.class.test",
                    "name": "Test",
                    "hitDieSides": 6,
                    "primaryAbilityIds": ["dnd5e2014.ability.wisdom"],
                    "requiresAllPrimaryAbilities": false,
                    "savingThrowProficiencyIds": [
                      "dnd5e2014.ability.wisdom",
                      "dnd5e2014.ability.charisma"
                    ],
                    "armorProficiencyCategories": [],
                    "proficientWithShields": false,
                    "weaponProficiencyCategories": [],
                    "weaponProficiencyIds": [],
                    "skillChoiceCount": 0,
                    "skillChoiceOptionIds": [],
                    "levelFeatures": [],
                    "spellSlotProgressionId":
                      "extension.spell-slot-progression.test",
                    "spellcastingAbilityId": "dnd5e2014.ability.wisdom",
                    "extraAttackProgressionId":
                      "extension.extra-attack-progression.test",
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
            "extension.spell-slot-progression.test",
            @class.SpellSlotProgressionId?.Value);
        Assert.Equal(
            "dnd5e2014.ability.wisdom",
            @class.SpellcastingAbilityId?.Value);
        Assert.Equal(
            "extension.extra-attack-progression.test",
            @class.ExtraAttackProgressionId?.Value);
    }

    [Fact]
    public void NullRootArray_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => ClassDefinitionLoader.LoadFromJson("null"));
    }

    [Fact]
    public void NullArrayElement_IsRejectedAsDataError()
    {
        InvalidDataException exception =
            Assert.Throws<InvalidDataException>(
                () => ClassDefinitionLoader.LoadFromJson("[null]"));

        Assert.Contains("index 0", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void UnknownProperty_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => ClassDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.class.test",
                    "name": "Test",
                    "hitDieSides": 10,
                    "primaryAbilityIds": [],
                    "requiresAllPrimaryAbilities": false,
                    "savingThrowProficiencyIds": [],
                    "armorProficiencyCategories": [],
                    "proficientWithShields": false,
                    "weaponProficiencyCategories": [],
                    "weaponProficiencyIds": [],
                    "skillChoiceCount": 0,
                    "skillChoiceOptionIds": [],
                    "levelFeatures": [],
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
            () => ClassDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.class.test",
                    "name": "Test",
                    "name": "Other",
                    "hitDieSides": 10,
                    "primaryAbilityIds": [],
                    "requiresAllPrimaryAbilities": false,
                    "savingThrowProficiencyIds": [],
                    "armorProficiencyCategories": [],
                    "proficientWithShields": false,
                    "weaponProficiencyCategories": [],
                    "weaponProficiencyIds": [],
                    "skillChoiceCount": 0,
                    "skillChoiceOptionIds": [],
                    "levelFeatures": [],
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void MissingRequiredHitDieSidesMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => ClassDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.class.test",
                    "name": "Test",
                    "primaryAbilityIds": [],
                    "requiresAllPrimaryAbilities": false,
                    "savingThrowProficiencyIds": [],
                    "armorProficiencyCategories": [],
                    "proficientWithShields": false,
                    "weaponProficiencyCategories": [],
                    "weaponProficiencyIds": [],
                    "skillChoiceCount": 0,
                    "skillChoiceOptionIds": [],
                    "levelFeatures": [],
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void NullRequiredIdMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => ClassDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": null,
                    "name": "Test",
                    "hitDieSides": 10,
                    "primaryAbilityIds": [],
                    "requiresAllPrimaryAbilities": false,
                    "savingThrowProficiencyIds": [],
                    "armorProficiencyCategories": [],
                    "proficientWithShields": false,
                    "weaponProficiencyCategories": [],
                    "weaponProficiencyIds": [],
                    "skillChoiceCount": 0,
                    "skillChoiceOptionIds": [],
                    "levelFeatures": [],
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void NullRequiredSourcesMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => ClassDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.class.test",
                    "name": "Test",
                    "hitDieSides": 10,
                    "primaryAbilityIds": [],
                    "requiresAllPrimaryAbilities": false,
                    "savingThrowProficiencyIds": [],
                    "armorProficiencyCategories": [],
                    "proficientWithShields": false,
                    "weaponProficiencyCategories": [],
                    "weaponProficiencyIds": [],
                    "skillChoiceCount": 0,
                    "skillChoiceOptionIds": [],
                    "levelFeatures": [],
                    "sources": null
                  }
                ]
                """));
    }

    [Fact]
    public void DuplicateIds_AreRejected()
    {
        string json = $"[{ValidClass},{ValidClass}]";

        Assert.Throws<InvalidDataException>(
            () => ClassDefinitionLoader.LoadFromJson(json));
    }
}
