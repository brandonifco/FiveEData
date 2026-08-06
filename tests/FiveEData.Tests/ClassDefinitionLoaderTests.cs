using FiveEData.Rules.Classes;
using FiveEData.Rules.Classes.Auras;
using FiveEData.Rules.Classes.BardicInspiration;
using FiveEData.Rules.Classes.ChannelDivinity;
using FiveEData.Rules.Classes.FontOfMagic;
using FiveEData.Rules.Classes.MysticArcanum;
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
          "wildShapeProgression": null,
          "auraOfProtection": null,
          "auraOfCourage": null,
          "bardicInspirationProgression": null,
          "channelDivinityProgression": null,
          "mysticArcanumProgression": null,
          "fontOfMagicConversion": null,
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
        Assert.Null(@class.WildShapeProgression);
        Assert.Null(@class.AuraOfProtection);
        Assert.Null(@class.AuraOfCourage);
        Assert.Null(@class.BardicInspirationProgression);
        Assert.Null(@class.ChannelDivinityProgression);
        Assert.Null(@class.MysticArcanumProgression);
        Assert.Null(@class.FontOfMagicConversion);
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
                    "wildShapeProgression": null,
                    "auraOfProtection": null,
                    "auraOfCourage": null,
                    "bardicInspirationProgression": null,
                    "channelDivinityProgression": null,
                    "mysticArcanumProgression": null,
                    "fontOfMagicConversion": null,
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
                    "wildShapeProgression": null,
                    "auraOfProtection": null,
                    "auraOfCourage": null,
                    "bardicInspirationProgression": null,
                    "channelDivinityProgression": null,
                    "mysticArcanumProgression": null,
                    "fontOfMagicConversion": null,
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
    public void ValidDefinition_LoadsWildShapeProgression()
    {
        ClassDefinition @class = Assert.Single(
            ClassDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.class.test",
                    "name": "Test",
                    "hitDieSides": 8,
                    "primaryAbilityIds": ["dnd5e2014.ability.wisdom"],
                    "requiresAllPrimaryAbilities": false,
                    "savingThrowProficiencyIds": [
                      "dnd5e2014.ability.intelligence",
                      "dnd5e2014.ability.wisdom"
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
                    "kiProgression": null,
                    "sorceryPointsProgression": null,
                    "wildShapeProgression": {
                      "formLimitsByLevel": [
                        {
                          "characterLevel": 2,
                          "maxChallengeRating": 0.25,
                          "allowsFlyingSpeed": false,
                          "allowsSwimmingSpeed": false
                        },
                        {
                          "characterLevel": 4,
                          "maxChallengeRating": 0.5,
                          "allowsFlyingSpeed": false,
                          "allowsSwimmingSpeed": true
                        }
                      ],
                      "usesPerRest": 2,
                      "recoversOnShortRest": true
                    },
                    "auraOfProtection": null,
                    "auraOfCourage": null,
                    "bardicInspirationProgression": null,
                    "channelDivinityProgression": null,
                    "mysticArcanumProgression": null,
                    "fontOfMagicConversion": null,
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

        Assert.NotNull(@class.WildShapeProgression);
        Assert.Equal(2, @class.WildShapeProgression!.FormLimitsByLevel.Count);
        Assert.Equal(
            0.25,
            @class.WildShapeProgression.FormLimitsByLevel[0]
                .MaxChallengeRating);
        Assert.False(
            @class.WildShapeProgression.FormLimitsByLevel[0]
                .AllowsSwimmingSpeed);
        Assert.True(
            @class.WildShapeProgression.FormLimitsByLevel[1]
                .AllowsSwimmingSpeed);
        Assert.Equal(2, @class.WildShapeProgression.UsesPerRest);
        Assert.True(@class.WildShapeProgression.RecoversOnShortRest);
    }

    [Fact]
    public void ValidDefinition_LoadsAuraOfProtectionAndAuraOfCourage()
    {
        ClassDefinition @class = Assert.Single(
            ClassDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.class.test",
                    "name": "Test",
                    "hitDieSides": 10,
                    "primaryAbilityIds": ["dnd5e2014.ability.strength"],
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
                    "spellSlotProgressionId": null,
                    "spellcastingAbilityId": null,
                    "extraAttackProgressionId": null,
                    "rageProgression": null,
                    "sneakAttackProgression": null,
                    "kiProgression": null,
                    "sorceryPointsProgression": null,
                    "wildShapeProgression": null,
                    "auraOfProtection": {
                      "range": {
                        "baseRangeFeet": 10,
                        "expandedRangeFeet": 30,
                        "expandedAtLevel": 18
                      },
                      "requiresConsciousness": true,
                      "savingThrowBonusMinimum": 1
                    },
                    "auraOfCourage": {
                      "range": {
                        "baseRangeFeet": 10,
                        "expandedRangeFeet": 30,
                        "expandedAtLevel": 18
                      },
                      "requiresConsciousness": true
                    },
                    "bardicInspirationProgression": null,
                    "channelDivinityProgression": null,
                    "mysticArcanumProgression": null,
                    "fontOfMagicConversion": null,
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

        AuraOfProtectionDetail auraOfProtection =
            @class.AuraOfProtection
            ?? throw new InvalidOperationException(
                "Expected an Aura of Protection.");
        AuraOfCourageDetail auraOfCourage =
            @class.AuraOfCourage
            ?? throw new InvalidOperationException(
                "Expected an Aura of Courage.");

        Assert.Equal(10, auraOfProtection.Range.BaseRangeFeet);
        Assert.Equal(30, auraOfProtection.Range.ExpandedRangeFeet);
        Assert.Equal(18, auraOfProtection.Range.ExpandedAtLevel);
        Assert.True(auraOfProtection.RequiresConsciousness);
        Assert.Equal(1, auraOfProtection.SavingThrowBonusMinimum);

        Assert.True(auraOfCourage.RequiresConsciousness);
    }

    [Fact]
    public void ValidDefinition_LoadsBardicInspirationProgression()
    {
        ClassDefinition @class = Assert.Single(
            ClassDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.class.test",
                    "name": "Test",
                    "hitDieSides": 8,
                    "primaryAbilityIds": ["dnd5e2014.ability.charisma"],
                    "requiresAllPrimaryAbilities": false,
                    "savingThrowProficiencyIds": [
                      "dnd5e2014.ability.dexterity",
                      "dnd5e2014.ability.charisma"
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
                    "kiProgression": null,
                    "sorceryPointsProgression": null,
                    "wildShapeProgression": null,
                    "auraOfProtection": null,
                    "auraOfCourage": null,
                    "bardicInspirationProgression": {
                      "dieByLevel": [
                        {
                          "characterLevel": 1,
                          "die": { "count": 1, "sides": 6 }
                        },
                        {
                          "characterLevel": 5,
                          "die": { "count": 1, "sides": 8 }
                        }
                      ],
                      "rangeFeet": 60,
                      "durationMinutes": 10
                    },
                    "channelDivinityProgression": null,
                    "mysticArcanumProgression": null,
                    "fontOfMagicConversion": null,
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

        BardicInspirationProgressionDetail bardicInspirationProgression =
            @class.BardicInspirationProgression
            ?? throw new InvalidOperationException(
                "Expected a Bardic Inspiration progression.");

        Assert.Equal(2, bardicInspirationProgression.DieByLevel.Count);
        Assert.Equal(
            6,
            bardicInspirationProgression.DieByLevel[0].Die.Sides);
        Assert.Equal(
            8,
            bardicInspirationProgression.DieByLevel[1].Die.Sides);
        Assert.Equal(60, bardicInspirationProgression.RangeFeet);
        Assert.Equal(10, bardicInspirationProgression.DurationMinutes);
    }

    [Fact]
    public void ValidDefinition_LoadsChannelDivinityProgression()
    {
        ClassDefinition @class = Assert.Single(
            ClassDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.class.test",
                    "name": "Test",
                    "hitDieSides": 8,
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
                    "spellSlotProgressionId": null,
                    "spellcastingAbilityId": null,
                    "extraAttackProgressionId": null,
                    "rageProgression": null,
                    "sneakAttackProgression": null,
                    "kiProgression": null,
                    "sorceryPointsProgression": null,
                    "wildShapeProgression": null,
                    "auraOfProtection": null,
                    "auraOfCourage": null,
                    "bardicInspirationProgression": null,
                    "channelDivinityProgression": {
                      "usesByLevel": [
                        { "characterLevel": 2, "usesPerRest": 1 },
                        { "characterLevel": 6, "usesPerRest": 2 }
                      ],
                      "recoversOnShortRest": true
                    },
                    "mysticArcanumProgression": null,
                    "fontOfMagicConversion": null,
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

        ChannelDivinityProgressionDetail channelDivinityProgression =
            @class.ChannelDivinityProgression
            ?? throw new InvalidOperationException(
                "Expected a Channel Divinity progression.");

        Assert.Equal(2, channelDivinityProgression.UsesByLevel.Count);
        Assert.Equal(
            1,
            channelDivinityProgression.UsesByLevel[0].UsesPerRest);
        Assert.Equal(
            2,
            channelDivinityProgression.UsesByLevel[1].UsesPerRest);
        Assert.True(channelDivinityProgression.RecoversOnShortRest);
    }

    [Fact]
    public void ValidDefinition_LoadsMysticArcanumProgression()
    {
        ClassDefinition @class = Assert.Single(
            ClassDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.class.test",
                    "name": "Test",
                    "hitDieSides": 8,
                    "primaryAbilityIds": ["dnd5e2014.ability.charisma"],
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
                    "spellSlotProgressionId": null,
                    "spellcastingAbilityId": null,
                    "extraAttackProgressionId": null,
                    "rageProgression": null,
                    "sneakAttackProgression": null,
                    "kiProgression": null,
                    "sorceryPointsProgression": null,
                    "wildShapeProgression": null,
                    "auraOfProtection": null,
                    "auraOfCourage": null,
                    "bardicInspirationProgression": null,
                    "channelDivinityProgression": null,
                    "mysticArcanumProgression": {
                      "arcanumByLevel": [
                        { "characterLevel": 11, "spellLevel": 6 },
                        { "characterLevel": 13, "spellLevel": 7 }
                      ],
                      "recoversOnShortRest": false
                    },
                    "fontOfMagicConversion": null,
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

        MysticArcanumProgressionDetail mysticArcanumProgression =
            @class.MysticArcanumProgression
            ?? throw new InvalidOperationException(
                "Expected a Mystic Arcanum progression.");

        Assert.Equal(2, mysticArcanumProgression.ArcanumByLevel.Count);
        Assert.Equal(
            6,
            mysticArcanumProgression.ArcanumByLevel[0].SpellLevel);
        Assert.Equal(
            7,
            mysticArcanumProgression.ArcanumByLevel[1].SpellLevel);
        Assert.False(mysticArcanumProgression.RecoversOnShortRest);
    }

    [Fact]
    public void ValidDefinition_LoadsFontOfMagicConversion()
    {
        ClassDefinition @class = Assert.Single(
            ClassDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.class.test",
                    "name": "Test",
                    "hitDieSides": 6,
                    "primaryAbilityIds": ["dnd5e2014.ability.charisma"],
                    "requiresAllPrimaryAbilities": false,
                    "savingThrowProficiencyIds": [
                      "dnd5e2014.ability.constitution",
                      "dnd5e2014.ability.charisma"
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
                    "kiProgression": null,
                    "sorceryPointsProgression": null,
                    "wildShapeProgression": null,
                    "auraOfProtection": null,
                    "auraOfCourage": null,
                    "bardicInspirationProgression": null,
                    "channelDivinityProgression": null,
                    "mysticArcanumProgression": null,
                    "fontOfMagicConversion": {
                      "slotCostByLevel": [
                        { "spellSlotLevel": 1, "sorceryPointCost": 2 },
                        { "spellSlotLevel": 2, "sorceryPointCost": 3 }
                      ]
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

        FontOfMagicConversionDetail fontOfMagicConversion =
            @class.FontOfMagicConversion
            ?? throw new InvalidOperationException(
                "Expected a Font of Magic conversion.");

        Assert.Equal(2, fontOfMagicConversion.SlotCostByLevel.Count);
        Assert.Equal(
            2,
            fontOfMagicConversion.SlotCostByLevel[0].SorceryPointCost);
        Assert.Equal(
            3,
            fontOfMagicConversion.SlotCostByLevel[1].SorceryPointCost);
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
