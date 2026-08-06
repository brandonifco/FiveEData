using FiveEData.Rules.Creatures.Races;
using FiveEData.Rules.Creatures.Races.Serialization;

namespace FiveEData.Tests;

public sealed class SubraceDefinitionLoaderTests
{
    private const string ValidSubrace =
        """
        {
          "id": "extension.subrace.test",
          "name": "Test",
          "raceId": "dnd5e2014.race.elf",
          "abilityScoreIncreases": [
            {
              "abilityId": "dnd5e2014.ability.wisdom",
              "bonus": 1
            }
          ],
          "speedFeet": 35,
          "additionalLanguageChoiceCount": 1,
          "traitRuleIds": ["dnd5e2014.race-rule.fleet-of-foot"],
          "darkvisionRangeFeet": null,
          "resistedDamageTypeIds": [],
          "hitPointBonusPerLevel": null,
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
        SubraceDefinition subrace = Assert.Single(
            SubraceDefinitionLoader.LoadFromJson($"[{ValidSubrace}]"));

        Assert.Equal("extension.subrace.test", subrace.Id.Value);
        Assert.Equal("Test", subrace.Name);
        Assert.Equal("dnd5e2014.race.elf", subrace.RaceId.Value);

        RaceAbilityScoreIncrease increase =
            Assert.Single(subrace.AbilityScoreIncreases);
        Assert.Equal("dnd5e2014.ability.wisdom", increase.AbilityId.Value);
        Assert.Equal(1, increase.Bonus);

        Assert.Equal(35, subrace.Speed?.Feet);
        Assert.Equal(1, subrace.AdditionalLanguageChoiceCount);
        Assert.Equal(
            "dnd5e2014.race-rule.fleet-of-foot",
            Assert.Single(subrace.TraitRuleIds).Value);
        Assert.Single(subrace.Sources);
    }

    [Fact]
    public void NullSpeedFeet_YieldsNoOverride()
    {
        SubraceDefinition subrace = Assert.Single(
            SubraceDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.subrace.test",
                    "name": "Test",
                    "raceId": "dnd5e2014.race.dwarf",
                    "abilityScoreIncreases": [],
                    "speedFeet": null,
                    "additionalLanguageChoiceCount": 0,
                    "traitRuleIds": [],
                    "darkvisionRangeFeet": null,
                    "resistedDamageTypeIds": [],
                    "hitPointBonusPerLevel": null,
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

        Assert.Null(subrace.Speed);
    }

    [Fact]
    public void NullRootArray_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => SubraceDefinitionLoader.LoadFromJson("null"));
    }

    [Fact]
    public void NullArrayElement_IsRejectedAsDataError()
    {
        InvalidDataException exception =
            Assert.Throws<InvalidDataException>(
                () => SubraceDefinitionLoader.LoadFromJson("[null]"));

        Assert.Contains("index 0", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void UnknownProperty_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => SubraceDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.subrace.test",
                    "name": "Test",
                    "raceId": "dnd5e2014.race.elf",
                    "abilityScoreIncreases": [],
                    "speedFeet": null,
                    "additionalLanguageChoiceCount": 0,
                    "traitRuleIds": [],
                    "darkvisionRangeFeet": null,
                    "resistedDamageTypeIds": [],
                    "hitPointBonusPerLevel": null,
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
            () => SubraceDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.subrace.test",
                    "name": "Test",
                    "name": "Other",
                    "raceId": "dnd5e2014.race.elf",
                    "abilityScoreIncreases": [],
                    "speedFeet": null,
                    "additionalLanguageChoiceCount": 0,
                    "traitRuleIds": [],
                    "darkvisionRangeFeet": null,
                    "resistedDamageTypeIds": [],
                    "hitPointBonusPerLevel": null,
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void MissingRequiredRaceIdMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => SubraceDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.subrace.test",
                    "name": "Test",
                    "abilityScoreIncreases": [],
                    "speedFeet": null,
                    "additionalLanguageChoiceCount": 0,
                    "traitRuleIds": [],
                    "darkvisionRangeFeet": null,
                    "resistedDamageTypeIds": [],
                    "hitPointBonusPerLevel": null,
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void MissingRequiredSpeedFeetMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => SubraceDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.subrace.test",
                    "name": "Test",
                    "raceId": "dnd5e2014.race.elf",
                    "abilityScoreIncreases": [],
                    "additionalLanguageChoiceCount": 0,
                    "traitRuleIds": [],
                    "darkvisionRangeFeet": null,
                    "resistedDamageTypeIds": [],
                    "hitPointBonusPerLevel": null,
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void NullRequiredIdMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => SubraceDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": null,
                    "name": "Test",
                    "raceId": "dnd5e2014.race.elf",
                    "abilityScoreIncreases": [],
                    "speedFeet": null,
                    "additionalLanguageChoiceCount": 0,
                    "traitRuleIds": [],
                    "darkvisionRangeFeet": null,
                    "resistedDamageTypeIds": [],
                    "hitPointBonusPerLevel": null,
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void NullRequiredRaceIdMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => SubraceDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.subrace.test",
                    "name": "Test",
                    "raceId": null,
                    "abilityScoreIncreases": [],
                    "speedFeet": null,
                    "additionalLanguageChoiceCount": 0,
                    "traitRuleIds": [],
                    "darkvisionRangeFeet": null,
                    "resistedDamageTypeIds": [],
                    "hitPointBonusPerLevel": null,
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void NullRequiredSourcesMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => SubraceDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.subrace.test",
                    "name": "Test",
                    "raceId": "dnd5e2014.race.elf",
                    "abilityScoreIncreases": [],
                    "speedFeet": null,
                    "additionalLanguageChoiceCount": 0,
                    "traitRuleIds": [],
                    "darkvisionRangeFeet": null,
                    "resistedDamageTypeIds": [],
                    "hitPointBonusPerLevel": null,
                    "sources": null
                  }
                ]
                """));
    }

    [Fact]
    public void DuplicateIds_AreRejected()
    {
        string json = $"[{ValidSubrace},{ValidSubrace}]";

        Assert.Throws<InvalidDataException>(
            () => SubraceDefinitionLoader.LoadFromJson(json));
    }
}
