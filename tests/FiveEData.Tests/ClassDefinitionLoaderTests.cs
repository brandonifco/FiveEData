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
          "rageProgression": null,
          "sneakAttackProgression": null,
          "kiProgression": null,
          "sorceryPointsProgression": null,
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
        Assert.Null(@class.RageProgression);
        Assert.Null(@class.SneakAttackProgression);
        Assert.Null(@class.KiProgression);
        Assert.Null(@class.SorceryPointsProgression);
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
                    "rageProgression": {
                      "usesByLevel": [
                        { "characterLevel": 1, "usesPerLongRest": 2 },
                        { "characterLevel": 20, "usesPerLongRest": null }
                      ],
                      "damageBonusByLevel": [
                        { "characterLevel": 1, "bonus": 2 }
                      ],
                      "durationMinutes": 1,
                      "resistedDamageTypeIds": [
                        "dnd5e2014.damage-type.bludgeoning"
                      ],
                      "requiresNotWearingHeavyArmor": true
                    },
                    "sneakAttackProgression": {
                      "diceByLevel": [
                        {
                          "characterLevel": 1,
                          "damage": { "count": 1, "sides": 6 }
                        },
                        {
                          "characterLevel": 3,
                          "damage": { "count": 2, "sides": 6 }
                        }
                      ],
                      "oncePerTurn": true,
                      "requiresFinesseOrRangedWeapon": true
                    },
                    "kiProgression": null,
                    "sorceryPointsProgression": null,
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

        Assert.NotNull(@class.RageProgression);
        Assert.Equal(2, @class.RageProgression!.UsesByLevel.Count);
        Assert.Equal(
            2,
            @class.RageProgression.UsesByLevel[0].UsesPerLongRest);
        Assert.Null(
            @class.RageProgression.UsesByLevel[1].UsesPerLongRest);
        Assert.Equal(
            2,
            Assert.Single(@class.RageProgression.DamageBonusByLevel).Bonus);
        Assert.Equal(1, @class.RageProgression.DurationMinutes);
        Assert.Equal(
            "dnd5e2014.damage-type.bludgeoning",
            Assert.Single(
                @class.RageProgression.ResistedDamageTypeIds).Value);
        Assert.True(@class.RageProgression.RequiresNotWearingHeavyArmor);

        Assert.NotNull(@class.SneakAttackProgression);
        Assert.Equal(
            2,
            @class.SneakAttackProgression!.DiceByLevel.Count);
        Assert.Equal(
            1,
            @class.SneakAttackProgression.DiceByLevel[0].Damage.Count);
        Assert.Equal(
            6,
            @class.SneakAttackProgression.DiceByLevel[0].Damage.Sides);
        Assert.Equal(
            2,
            @class.SneakAttackProgression.DiceByLevel[1].Damage.Count);
        Assert.True(@class.SneakAttackProgression.OncePerTurn);
        Assert.True(
            @class.SneakAttackProgression.RequiresFinesseOrRangedWeapon);
    }

    [Fact]
    public void ValidDefinition_LoadsKiAndSorceryPointsProgressions()
    {
        ClassDefinition @class = Assert.Single(
            ClassDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.class.test",
                    "name": "Test",
                    "hitDieSides": 8,
                    "primaryAbilityIds": ["dnd5e2014.ability.dexterity"],
                    "requiresAllPrimaryAbilities": false,
                    "savingThrowProficiencyIds": [
                      "dnd5e2014.ability.strength",
                      "dnd5e2014.ability.dexterity"
                    ],
                    "armorProficiencyCategories": [],
                    "proficientWithShields": false,
                    "weaponProficiencyCategories": [],
                    "weaponProficiencyIds": [],
                    "skillChoiceCount": 0,
                    "skillChoiceOptionIds": [],
                    "levelFeatures": [],
                    "spellSlotProgressionId": null,
                    "spellcastingAbilityId": null,
                    "extraAttackProgressionId": null,
                    "rageProgression": null,
                    "sneakAttackProgression": null,
                    "kiProgression": {
                      "pointsByLevel": [
                        { "characterLevel": 2, "points": 2 },
                        { "characterLevel": 3, "points": 3 }
                      ],
                      "recoversOnShortRest": true
                    },
                    "sorceryPointsProgression": {
                      "pointsByLevel": [
                        { "characterLevel": 2, "points": 2 },
                        { "characterLevel": 3, "points": 3 }
                      ],
                      "recoversOnShortRest": false
                    },
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

        Assert.NotNull(@class.KiProgression);
        Assert.Equal(2, @class.KiProgression!.PointsByLevel.Count);
        Assert.Equal(2, @class.KiProgression.PointsByLevel[0].Points);
        Assert.True(@class.KiProgression.RecoversOnShortRest);

        Assert.NotNull(@class.SorceryPointsProgression);
        Assert.Equal(2, @class.SorceryPointsProgression!.PointsByLevel.Count);
        Assert.Equal(3, @class.SorceryPointsProgression.PointsByLevel[1].Points);
        Assert.False(@class.SorceryPointsProgression.RecoversOnShortRest);
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
