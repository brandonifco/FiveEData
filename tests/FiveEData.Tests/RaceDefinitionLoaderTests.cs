using FiveEData.Rules.Creatures.Races;
using FiveEData.Rules.Creatures.Races.Serialization;

namespace FiveEData.Tests;

public sealed class RaceDefinitionLoaderTests
{
    private const string ValidRace =
        """
        {
          "id": "extension.race.test",
          "name": "Test",
          "size": "dnd5e2014.creature-size.medium",
          "speedFeet": 30,
          "abilityScoreIncreases": [
            {
              "abilityId": "dnd5e2014.ability.strength",
              "bonus": 2
            }
          ],
          "choosableAbilityScoreIncreaseCount": 0,
          "languageIds": ["dnd5e2014.language.common"],
          "additionalLanguageChoiceCount": 0,
          "traitRuleIds": ["dnd5e2014.race-rule.darkvision"],
          "darkvisionRangeFeet": null,
          "resistedDamageTypeIds": [],
          "tranceDurationHours": null,
          "breathWeaponProgression": null,
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
        RaceDefinition race = Assert.Single(
            RaceDefinitionLoader.LoadFromJson($"[{ValidRace}]"));

        Assert.Equal("extension.race.test", race.Id.Value);
        Assert.Equal("Test", race.Name);
        Assert.Equal(
            "dnd5e2014.creature-size.medium",
            race.Size.Value);
        Assert.Equal(30, race.Speed.Feet);

        RaceAbilityScoreIncrease increase =
            Assert.Single(race.AbilityScoreIncreases);
        Assert.Equal(
            "dnd5e2014.ability.strength",
            increase.AbilityId.Value);
        Assert.Equal(2, increase.Bonus);

        Assert.Equal(0, race.ChoosableAbilityScoreIncreaseCount);
        Assert.Equal(
            "dnd5e2014.language.common",
            Assert.Single(race.LanguageIds).Value);
        Assert.Equal(0, race.AdditionalLanguageChoiceCount);
        Assert.Equal(
            "dnd5e2014.race-rule.darkvision",
            Assert.Single(race.TraitRuleIds).Value);
        Assert.Single(race.Sources);
    }

    [Fact]
    public void NullRootArray_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => RaceDefinitionLoader.LoadFromJson("null"));
    }

    [Fact]
    public void NullArrayElement_IsRejectedAsDataError()
    {
        InvalidDataException exception =
            Assert.Throws<InvalidDataException>(
                () => RaceDefinitionLoader.LoadFromJson("[null]"));

        Assert.Contains("index 0", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void UnknownProperty_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => RaceDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.race.test",
                    "name": "Test",
                    "size": "dnd5e2014.creature-size.medium",
                    "speedFeet": 30,
                    "abilityScoreIncreases": [],
                    "choosableAbilityScoreIncreaseCount": 0,
                    "languageIds": [],
                    "additionalLanguageChoiceCount": 0,
                    "traitRuleIds": [],
                    "darkvisionRangeFeet": null,
                    "resistedDamageTypeIds": [],
                    "tranceDurationHours": null,
                    "breathWeaponProgression": null,
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
            () => RaceDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.race.test",
                    "name": "Test",
                    "name": "Other",
                    "size": "dnd5e2014.creature-size.medium",
                    "speedFeet": 30,
                    "abilityScoreIncreases": [],
                    "choosableAbilityScoreIncreaseCount": 0,
                    "languageIds": [],
                    "additionalLanguageChoiceCount": 0,
                    "traitRuleIds": [],
                    "darkvisionRangeFeet": null,
                    "resistedDamageTypeIds": [],
                    "tranceDurationHours": null,
                    "breathWeaponProgression": null,
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void MissingRequiredSizeMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => RaceDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.race.test",
                    "name": "Test",
                    "speedFeet": 30,
                    "abilityScoreIncreases": [],
                    "choosableAbilityScoreIncreaseCount": 0,
                    "languageIds": [],
                    "additionalLanguageChoiceCount": 0,
                    "traitRuleIds": [],
                    "darkvisionRangeFeet": null,
                    "resistedDamageTypeIds": [],
                    "tranceDurationHours": null,
                    "breathWeaponProgression": null,
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void NullRequiredIdMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => RaceDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": null,
                    "name": "Test",
                    "size": "dnd5e2014.creature-size.medium",
                    "speedFeet": 30,
                    "abilityScoreIncreases": [],
                    "choosableAbilityScoreIncreaseCount": 0,
                    "languageIds": [],
                    "additionalLanguageChoiceCount": 0,
                    "traitRuleIds": [],
                    "darkvisionRangeFeet": null,
                    "resistedDamageTypeIds": [],
                    "tranceDurationHours": null,
                    "breathWeaponProgression": null,
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void NullRequiredNameMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => RaceDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.race.test",
                    "name": null,
                    "size": "dnd5e2014.creature-size.medium",
                    "speedFeet": 30,
                    "abilityScoreIncreases": [],
                    "choosableAbilityScoreIncreaseCount": 0,
                    "languageIds": [],
                    "additionalLanguageChoiceCount": 0,
                    "traitRuleIds": [],
                    "darkvisionRangeFeet": null,
                    "resistedDamageTypeIds": [],
                    "tranceDurationHours": null,
                    "breathWeaponProgression": null,
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void NullRequiredSourcesMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => RaceDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.race.test",
                    "name": "Test",
                    "size": "dnd5e2014.creature-size.medium",
                    "speedFeet": 30,
                    "abilityScoreIncreases": [],
                    "choosableAbilityScoreIncreaseCount": 0,
                    "languageIds": [],
                    "additionalLanguageChoiceCount": 0,
                    "traitRuleIds": [],
                    "darkvisionRangeFeet": null,
                    "resistedDamageTypeIds": [],
                    "tranceDurationHours": null,
                    "breathWeaponProgression": null,
                    "sources": null
                  }
                ]
                """));
    }

    [Fact]
    public void DuplicateIds_AreRejected()
    {
        string json = $"[{ValidRace},{ValidRace}]";

        Assert.Throws<InvalidDataException>(
            () => RaceDefinitionLoader.LoadFromJson(json));
    }
}
