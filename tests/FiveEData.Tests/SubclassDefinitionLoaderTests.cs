using FiveEData.Rules.Classes;
using FiveEData.Rules.Classes.Auras;
using FiveEData.Rules.Classes.CircleForms;
using FiveEData.Rules.Classes.DivineStrike;
using FiveEData.Rules.Classes.DraconicResilience;
using FiveEData.Rules.Classes.MagicalSecrets;
using FiveEData.Rules.Classes.Portent;
using FiveEData.Rules.Classes.Serialization;

namespace FiveEData.Tests;

public sealed class SubclassDefinitionLoaderTests
{
    private const string ValidSubclass =
        """
        {
          "id": "extension.subclass.test",
          "name": "Test",
          "classId": "dnd5e2014.class.fighter",
          "chosenAtLevel": 3,
          "levelFeatures": [
            {
              "level": 3,
              "featureRuleId": "dnd5e2014.class-rule.test"
            }
          ],
          "spellSlotProgressionId": null,
          "spellcastingAbilityId": null,
          "divineStrikeProgression": null,
          "circleFormsProgression": null,
          "auraOfDevotion": null,
          "auraOfWarding": null,
          "combatSuperiorityProgression": null,
          "discipleOfTheElementsProgression": null,
          "magicalSecretsProgression": null,
          "portentProgression": null,
          "draconicResilience": null,
          "improvedCriticalProgression": null,
          "shadowStep": null,
          "hurlThroughHell": null,
          "wrathOfTheStorm": null,
          "thunderboltStrike": null,
          "shadowArtsKiCost": null,
          "quiveringPalmKiCost": null,
          "draconicPresenceSorceryPointCost": null,
          "bendLuck": null,
          "wardingFlare": null,
          "warPriestUsesPerRest": null,
          "innateSpellGrants": [],
          "frenzy": null,
          "mindlessRageImmuneConditionIds": [],
          "intimidatingPresence": null,
          "secondStoryWork": null,
          "assassinate": null,
          "infiltrationExpertise": null,
          "impostorRequiredStudyHours": null,
          "deathStrike": null,
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
        SubclassDefinition subclass = Assert.Single(
            SubclassDefinitionLoader.LoadFromJson($"[{ValidSubclass}]"));

        Assert.Equal("extension.subclass.test", subclass.Id.Value);
        Assert.Equal("Test", subclass.Name);
        Assert.Equal("dnd5e2014.class.fighter", subclass.ClassId.Value);
        Assert.Equal(3, subclass.ChosenAtLevel);

        ClassLevelFeature feature = Assert.Single(subclass.LevelFeatures);
        Assert.Equal(3, feature.Level);
        Assert.Equal(
            "dnd5e2014.class-rule.test",
            feature.FeatureRuleId.Value);

        Assert.Null(subclass.SpellSlotProgressionId);
        Assert.Null(subclass.SpellcastingAbilityId);
        Assert.Null(subclass.DivineStrikeProgression);
        Assert.Null(subclass.CircleFormsProgression);
        Assert.Null(subclass.AuraOfDevotion);
        Assert.Null(subclass.AuraOfWarding);
        Assert.Single(subclass.Sources);
    }

    [Fact]
    public void ValidDefinition_LoadsSpellcastingFields()
    {
        SubclassDefinition subclass = Assert.Single(
            SubclassDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.subclass.test",
                    "name": "Test",
                    "classId": "dnd5e2014.class.fighter",
                    "chosenAtLevel": 3,
                    "levelFeatures": [],
                    "spellSlotProgressionId":
                      "extension.spell-slot-progression.test",
                    "spellcastingAbilityId": "dnd5e2014.ability.intelligence",
                    "divineStrikeProgression": null,
                    "circleFormsProgression": null,
                    "auraOfDevotion": null,
                    "auraOfWarding": null,
                    "combatSuperiorityProgression": null,
                    "discipleOfTheElementsProgression": null,
                    "magicalSecretsProgression": null,
                    "portentProgression": null,
                    "draconicResilience": null,
                    "improvedCriticalProgression": null,
                    "shadowStep": null,
                    "hurlThroughHell": null,
                    "wrathOfTheStorm": null,
                    "thunderboltStrike": null,
                    "shadowArtsKiCost": null,
                    "quiveringPalmKiCost": null,
                    "draconicPresenceSorceryPointCost": null,
                    "bendLuck": null,
                    "wardingFlare": null,
                    "warPriestUsesPerRest": null,
                    "innateSpellGrants": [],
                    "frenzy": null,
                    "mindlessRageImmuneConditionIds": [],
                    "intimidatingPresence": null,
                    "secondStoryWork": null,
                    "assassinate": null,
                    "infiltrationExpertise": null,
                    "impostorRequiredStudyHours": null,
                    "deathStrike": null,
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
            subclass.SpellSlotProgressionId?.Value);
        Assert.Equal(
            "dnd5e2014.ability.intelligence",
            subclass.SpellcastingAbilityId?.Value);
    }

    [Fact]
    public void ValidDefinition_LoadsDivineStrikeProgression()
    {
        SubclassDefinition subclass = Assert.Single(
            SubclassDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.subclass.test",
                    "name": "Test",
                    "classId": "dnd5e2014.class.fighter",
                    "chosenAtLevel": 3,
                    "levelFeatures": [],
                    "spellSlotProgressionId": null,
                    "spellcastingAbilityId": null,
                    "divineStrikeProgression": {
                      "damageByLevel": [
                        {
                          "characterLevel": 8,
                          "damage": { "count": 1, "sides": 8 }
                        },
                        {
                          "characterLevel": 14,
                          "damage": { "count": 2, "sides": 8 }
                        }
                      ],
                      "fixedDamageTypeId": "dnd5e2014.damage-type.radiant",
                      "choosableDamageTypeIds": null,
                      "matchesWeaponDamageType": false
                    },
                    "circleFormsProgression": null,
                    "auraOfDevotion": null,
                    "auraOfWarding": null,
                    "combatSuperiorityProgression": null,
                    "discipleOfTheElementsProgression": null,
                    "magicalSecretsProgression": null,
                    "portentProgression": null,
                    "draconicResilience": null,
                    "improvedCriticalProgression": null,
                    "shadowStep": null,
                    "hurlThroughHell": null,
                    "wrathOfTheStorm": null,
                    "thunderboltStrike": null,
                    "shadowArtsKiCost": null,
                    "quiveringPalmKiCost": null,
                    "draconicPresenceSorceryPointCost": null,
                    "bendLuck": null,
                    "wardingFlare": null,
                    "warPriestUsesPerRest": null,
                    "innateSpellGrants": [],
                    "frenzy": null,
                    "mindlessRageImmuneConditionIds": [],
                    "intimidatingPresence": null,
                    "secondStoryWork": null,
                    "assassinate": null,
                    "infiltrationExpertise": null,
                    "impostorRequiredStudyHours": null,
                    "deathStrike": null,
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

        DivineStrikeProgressionDetail divineStrikeProgression =
            subclass.DivineStrikeProgression
            ?? throw new InvalidOperationException(
                "Expected a Divine Strike progression.");

        Assert.Equal(2, divineStrikeProgression.DamageByLevel.Count);
        Assert.Equal(
            "dnd5e2014.damage-type.radiant",
            divineStrikeProgression.FixedDamageTypeId?.Value);
        Assert.Null(divineStrikeProgression.ChoosableDamageTypeIds);
        Assert.False(divineStrikeProgression.MatchesWeaponDamageType);
    }

    [Fact]
    public void ValidDefinition_LoadsCircleFormsProgression()
    {
        SubclassDefinition subclass = Assert.Single(
            SubclassDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.subclass.test",
                    "name": "Test",
                    "classId": "dnd5e2014.class.druid",
                    "chosenAtLevel": 2,
                    "levelFeatures": [],
                    "spellSlotProgressionId": null,
                    "spellcastingAbilityId": null,
                    "divineStrikeProgression": null,
                    "circleFormsProgression": {
                      "maxChallengeRatingByLevel": [
                        { "characterLevel": 2, "maxChallengeRating": 1.0 },
                        { "characterLevel": 6, "maxChallengeRating": 2.0 }
                      ]
                    },
                    "auraOfDevotion": null,
                    "auraOfWarding": null,
                    "combatSuperiorityProgression": null,
                    "discipleOfTheElementsProgression": null,
                    "magicalSecretsProgression": null,
                    "portentProgression": null,
                    "draconicResilience": null,
                    "improvedCriticalProgression": null,
                    "shadowStep": null,
                    "hurlThroughHell": null,
                    "wrathOfTheStorm": null,
                    "thunderboltStrike": null,
                    "shadowArtsKiCost": null,
                    "quiveringPalmKiCost": null,
                    "draconicPresenceSorceryPointCost": null,
                    "bendLuck": null,
                    "wardingFlare": null,
                    "warPriestUsesPerRest": null,
                    "innateSpellGrants": [],
                    "frenzy": null,
                    "mindlessRageImmuneConditionIds": [],
                    "intimidatingPresence": null,
                    "secondStoryWork": null,
                    "assassinate": null,
                    "infiltrationExpertise": null,
                    "impostorRequiredStudyHours": null,
                    "deathStrike": null,
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

        CircleFormsProgressionDetail circleFormsProgression =
            subclass.CircleFormsProgression
            ?? throw new InvalidOperationException(
                "Expected a Circle Forms progression.");

        Assert.Equal(2, circleFormsProgression.MaxChallengeRatingByLevel.Count);
        Assert.Equal(
            1.0,
            circleFormsProgression.MaxChallengeRatingByLevel[0]
                .MaxChallengeRating);
        Assert.Equal(
            2.0,
            circleFormsProgression.MaxChallengeRatingByLevel[1]
                .MaxChallengeRating);
    }

    [Fact]
    public void ValidDefinition_LoadsAuraOfDevotionAndAuraOfWarding()
    {
        SubclassDefinition subclass = Assert.Single(
            SubclassDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.subclass.test",
                    "name": "Test",
                    "classId": "dnd5e2014.class.paladin",
                    "chosenAtLevel": 3,
                    "levelFeatures": [],
                    "spellSlotProgressionId": null,
                    "spellcastingAbilityId": null,
                    "divineStrikeProgression": null,
                    "circleFormsProgression": null,
                    "auraOfDevotion": {
                      "range": {
                        "baseRangeFeet": 10,
                        "expandedRangeFeet": 30,
                        "expandedAtLevel": 18
                      },
                      "requiresConsciousness": true
                    },
                    "auraOfWarding": {
                      "range": {
                        "baseRangeFeet": 10,
                        "expandedRangeFeet": 30,
                        "expandedAtLevel": 18
                      },
                      "requiresConsciousness": false
                    },
                    "combatSuperiorityProgression": null,
                    "discipleOfTheElementsProgression": null,
                    "magicalSecretsProgression": null,
                    "portentProgression": null,
                    "draconicResilience": null,
                    "improvedCriticalProgression": null,
                    "shadowStep": null,
                    "hurlThroughHell": null,
                    "wrathOfTheStorm": null,
                    "thunderboltStrike": null,
                    "shadowArtsKiCost": null,
                    "quiveringPalmKiCost": null,
                    "draconicPresenceSorceryPointCost": null,
                    "bendLuck": null,
                    "wardingFlare": null,
                    "warPriestUsesPerRest": null,
                    "innateSpellGrants": [],
                    "frenzy": null,
                    "mindlessRageImmuneConditionIds": [],
                    "intimidatingPresence": null,
                    "secondStoryWork": null,
                    "assassinate": null,
                    "infiltrationExpertise": null,
                    "impostorRequiredStudyHours": null,
                    "deathStrike": null,
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

        AuraOfDevotionDetail auraOfDevotion =
            subclass.AuraOfDevotion
            ?? throw new InvalidOperationException(
                "Expected an Aura of Devotion.");
        AuraOfWardingDetail auraOfWarding =
            subclass.AuraOfWarding
            ?? throw new InvalidOperationException(
                "Expected an Aura of Warding.");

        Assert.Equal(10, auraOfDevotion.Range.BaseRangeFeet);
        Assert.Equal(30, auraOfDevotion.Range.ExpandedRangeFeet);
        Assert.Equal(18, auraOfDevotion.Range.ExpandedAtLevel);
        Assert.True(auraOfDevotion.RequiresConsciousness);

        Assert.False(auraOfWarding.RequiresConsciousness);
    }

    [Fact]
    public void ValidDefinition_LoadsQuantizedSubclassFeatures()
    {
        SubclassDefinition subclass = Assert.Single(
            SubclassDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.subclass.test",
                    "name": "Test",
                    "classId": "dnd5e2014.class.wizard",
                    "chosenAtLevel": 2,
                    "levelFeatures": [],
                    "spellSlotProgressionId": null,
                    "spellcastingAbilityId": null,
                    "divineStrikeProgression": null,
                    "circleFormsProgression": null,
                    "auraOfDevotion": null,
                    "auraOfWarding": null,
                    "combatSuperiorityProgression": null,
                    "discipleOfTheElementsProgression": null,
                    "magicalSecretsProgression": {
                      "spellsKnownByLevel": [
                        { "characterLevel": 6, "spellsKnown": 2 }
                      ],
                      "countsAgainstSpellsKnown": false
                    },
                    "portentProgression": {
                      "foretellingRollsByLevel": [
                        { "characterLevel": 2, "foretellingRolls": 2 },
                        { "characterLevel": 14, "foretellingRolls": 3 }
                      ],
                      "oncePerTurn": true,
                      "recoversOnLongRest": true
                    },
                    "draconicResilience": {
                      "hitPointBonusPerLevel": 1,
                      "unarmoredBaseArmorClass": 13,
                      "unarmoredIncludesDexterityModifier": true
                    },
                    "improvedCriticalProgression": null,
                    "shadowStep": null,
                    "hurlThroughHell": null,
                    "wrathOfTheStorm": null,
                    "thunderboltStrike": null,
                    "shadowArtsKiCost": null,
                    "quiveringPalmKiCost": null,
                    "draconicPresenceSorceryPointCost": null,
                    "bendLuck": null,
                    "wardingFlare": null,
                    "warPriestUsesPerRest": null,
                    "innateSpellGrants": [],
                    "frenzy": null,
                    "mindlessRageImmuneConditionIds": [],
                    "intimidatingPresence": null,
                    "secondStoryWork": null,
                    "assassinate": null,
                    "infiltrationExpertise": null,
                    "impostorRequiredStudyHours": null,
                    "deathStrike": null,
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

        MagicalSecretsProgressionDetail magicalSecretsProgression =
            subclass.MagicalSecretsProgression
            ?? throw new InvalidOperationException(
                "Expected a Magical Secrets progression.");
        Assert.Equal(
            6,
            Assert.Single(magicalSecretsProgression.SpellsKnownByLevel)
                .CharacterLevel);
        Assert.False(magicalSecretsProgression.CountsAgainstSpellsKnown);

        PortentProgressionDetail portentProgression =
            subclass.PortentProgression
            ?? throw new InvalidOperationException(
                "Expected a Portent progression.");
        Assert.Equal(2, portentProgression.ForetellingRollsByLevel.Count);
        Assert.Equal(
            3,
            portentProgression.ForetellingRollsByLevel[1].ForetellingRolls);
        Assert.True(portentProgression.OncePerTurn);
        Assert.True(portentProgression.RecoversOnLongRest);

        DraconicResilienceDetail draconicResilience =
            subclass.DraconicResilience
            ?? throw new InvalidOperationException(
                "Expected Draconic Resilience.");
        Assert.Equal(1, draconicResilience.HitPointBonusPerLevel);
        Assert.Equal(
            13,
            draconicResilience.UnarmoredArmorClass.BaseArmorClass);
        Assert.True(
            draconicResilience.UnarmoredArmorClass
                .IncludesDexterityModifier);
    }

    [Fact]
    public void NullRootArray_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => SubclassDefinitionLoader.LoadFromJson("null"));
    }

    [Fact]
    public void NullArrayElement_IsRejectedAsDataError()
    {
        InvalidDataException exception =
            Assert.Throws<InvalidDataException>(
                () => SubclassDefinitionLoader.LoadFromJson("[null]"));

        Assert.Contains("index 0", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void UnknownProperty_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => SubclassDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.subclass.test",
                    "name": "Test",
                    "classId": "dnd5e2014.class.fighter",
                    "chosenAtLevel": 3,
                    "levelFeatures": [],
                    "magicalSecretsProgression": null,
                    "portentProgression": null,
                    "draconicResilience": null,
                    "improvedCriticalProgression": null,
                    "shadowStep": null,
                    "hurlThroughHell": null,
                    "wrathOfTheStorm": null,
                    "thunderboltStrike": null,
                    "shadowArtsKiCost": null,
                    "quiveringPalmKiCost": null,
                    "draconicPresenceSorceryPointCost": null,
                    "bendLuck": null,
                    "wardingFlare": null,
                    "warPriestUsesPerRest": null,
                    "innateSpellGrants": [],
                    "frenzy": null,
                    "mindlessRageImmuneConditionIds": [],
                    "intimidatingPresence": null,
                    "secondStoryWork": null,
                    "assassinate": null,
                    "infiltrationExpertise": null,
                    "impostorRequiredStudyHours": null,
                    "deathStrike": null,
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
            () => SubclassDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.subclass.test",
                    "name": "Test",
                    "name": "Other",
                    "classId": "dnd5e2014.class.fighter",
                    "chosenAtLevel": 3,
                    "levelFeatures": [],
                    "magicalSecretsProgression": null,
                    "portentProgression": null,
                    "draconicResilience": null,
                    "improvedCriticalProgression": null,
                    "shadowStep": null,
                    "hurlThroughHell": null,
                    "wrathOfTheStorm": null,
                    "thunderboltStrike": null,
                    "shadowArtsKiCost": null,
                    "quiveringPalmKiCost": null,
                    "draconicPresenceSorceryPointCost": null,
                    "bendLuck": null,
                    "wardingFlare": null,
                    "warPriestUsesPerRest": null,
                    "innateSpellGrants": [],
                    "frenzy": null,
                    "mindlessRageImmuneConditionIds": [],
                    "intimidatingPresence": null,
                    "secondStoryWork": null,
                    "assassinate": null,
                    "infiltrationExpertise": null,
                    "impostorRequiredStudyHours": null,
                    "deathStrike": null,
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void MissingRequiredClassIdMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => SubclassDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.subclass.test",
                    "name": "Test",
                    "chosenAtLevel": 3,
                    "levelFeatures": [],
                    "magicalSecretsProgression": null,
                    "portentProgression": null,
                    "draconicResilience": null,
                    "improvedCriticalProgression": null,
                    "shadowStep": null,
                    "hurlThroughHell": null,
                    "wrathOfTheStorm": null,
                    "thunderboltStrike": null,
                    "shadowArtsKiCost": null,
                    "quiveringPalmKiCost": null,
                    "draconicPresenceSorceryPointCost": null,
                    "bendLuck": null,
                    "wardingFlare": null,
                    "warPriestUsesPerRest": null,
                    "innateSpellGrants": [],
                    "frenzy": null,
                    "mindlessRageImmuneConditionIds": [],
                    "intimidatingPresence": null,
                    "secondStoryWork": null,
                    "assassinate": null,
                    "infiltrationExpertise": null,
                    "impostorRequiredStudyHours": null,
                    "deathStrike": null,
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void NullRequiredIdMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => SubclassDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": null,
                    "name": "Test",
                    "classId": "dnd5e2014.class.fighter",
                    "chosenAtLevel": 3,
                    "levelFeatures": [],
                    "magicalSecretsProgression": null,
                    "portentProgression": null,
                    "draconicResilience": null,
                    "improvedCriticalProgression": null,
                    "shadowStep": null,
                    "hurlThroughHell": null,
                    "wrathOfTheStorm": null,
                    "thunderboltStrike": null,
                    "shadowArtsKiCost": null,
                    "quiveringPalmKiCost": null,
                    "draconicPresenceSorceryPointCost": null,
                    "bendLuck": null,
                    "wardingFlare": null,
                    "warPriestUsesPerRest": null,
                    "innateSpellGrants": [],
                    "frenzy": null,
                    "mindlessRageImmuneConditionIds": [],
                    "intimidatingPresence": null,
                    "secondStoryWork": null,
                    "assassinate": null,
                    "infiltrationExpertise": null,
                    "impostorRequiredStudyHours": null,
                    "deathStrike": null,
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void NullRequiredClassIdMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => SubclassDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.subclass.test",
                    "name": "Test",
                    "classId": null,
                    "chosenAtLevel": 3,
                    "levelFeatures": [],
                    "magicalSecretsProgression": null,
                    "portentProgression": null,
                    "draconicResilience": null,
                    "improvedCriticalProgression": null,
                    "shadowStep": null,
                    "hurlThroughHell": null,
                    "wrathOfTheStorm": null,
                    "thunderboltStrike": null,
                    "shadowArtsKiCost": null,
                    "quiveringPalmKiCost": null,
                    "draconicPresenceSorceryPointCost": null,
                    "bendLuck": null,
                    "wardingFlare": null,
                    "warPriestUsesPerRest": null,
                    "innateSpellGrants": [],
                    "frenzy": null,
                    "mindlessRageImmuneConditionIds": [],
                    "intimidatingPresence": null,
                    "secondStoryWork": null,
                    "assassinate": null,
                    "infiltrationExpertise": null,
                    "impostorRequiredStudyHours": null,
                    "deathStrike": null,
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void NullRequiredSourcesMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => SubclassDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.subclass.test",
                    "name": "Test",
                    "classId": "dnd5e2014.class.fighter",
                    "chosenAtLevel": 3,
                    "levelFeatures": [],
                    "magicalSecretsProgression": null,
                    "portentProgression": null,
                    "draconicResilience": null,
                    "improvedCriticalProgression": null,
                    "shadowStep": null,
                    "hurlThroughHell": null,
                    "wrathOfTheStorm": null,
                    "thunderboltStrike": null,
                    "shadowArtsKiCost": null,
                    "quiveringPalmKiCost": null,
                    "draconicPresenceSorceryPointCost": null,
                    "bendLuck": null,
                    "wardingFlare": null,
                    "warPriestUsesPerRest": null,
                    "innateSpellGrants": [],
                    "frenzy": null,
                    "mindlessRageImmuneConditionIds": [],
                    "intimidatingPresence": null,
                    "secondStoryWork": null,
                    "assassinate": null,
                    "infiltrationExpertise": null,
                    "impostorRequiredStudyHours": null,
                    "deathStrike": null,
                    "sources": null
                  }
                ]
                """));
    }

    [Fact]
    public void DuplicateIds_AreRejected()
    {
        string json = $"[{ValidSubclass},{ValidSubclass}]";

        Assert.Throws<InvalidDataException>(
            () => SubclassDefinitionLoader.LoadFromJson(json));
    }
}
