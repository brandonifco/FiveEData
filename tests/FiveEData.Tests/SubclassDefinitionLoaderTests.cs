using FiveEData.Rules.Classes;
using FiveEData.Rules.Classes.DivineStrike;
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
