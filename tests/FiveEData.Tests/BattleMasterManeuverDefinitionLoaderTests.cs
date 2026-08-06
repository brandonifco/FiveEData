using FiveEData.Rules.Classes.BattleMasterManeuvers;
using FiveEData.Rules.Classes.BattleMasterManeuvers.Serialization;

namespace FiveEData.Tests;

public sealed class BattleMasterManeuverDefinitionLoaderTests
{
    [Fact]
    public void ValidDefinition_LoadsStrictlyWithoutSavingThrow()
    {
        BattleMasterManeuverDefinition definition = Assert.Single(
            BattleMasterManeuverDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.battle-master-maneuver.test",
                    "name": "Test",
                    "effectTarget": "DamageRoll",
                    "savingThrowAbilityId": null,
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
            "extension.battle-master-maneuver.test",
            definition.Id.Value);
        Assert.Equal("Test", definition.Name);
        Assert.Equal(
            BattleMasterManeuverEffectTarget.DamageRoll,
            definition.EffectTarget);
        Assert.Null(definition.SavingThrowAbilityId);
        Assert.Single(definition.Sources);
    }

    [Fact]
    public void ValidDefinition_LoadsSavingThrowAbilityWhenPresent()
    {
        BattleMasterManeuverDefinition definition = Assert.Single(
            BattleMasterManeuverDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.battle-master-maneuver.test",
                    "name": "Test",
                    "effectTarget": "DamageRoll",
                    "savingThrowAbilityId": "dnd5e2014.ability.strength",
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
            "dnd5e2014.ability.strength",
            definition.SavingThrowAbilityId?.Value);
    }

    [Fact]
    public void NullRootArray_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => BattleMasterManeuverDefinitionLoader.LoadFromJson("null"));
    }

    [Fact]
    public void NullArrayElement_IsRejectedAsDataError()
    {
        InvalidDataException exception =
            Assert.Throws<InvalidDataException>(
                () => BattleMasterManeuverDefinitionLoader.LoadFromJson(
                    "[null]"));

        Assert.Contains(
            "index 0",
            exception.Message,
            StringComparison.Ordinal);
    }

    [Fact]
    public void UnknownProperty_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => BattleMasterManeuverDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.battle-master-maneuver.test",
                    "name": "Test",
                    "effectTarget": "DamageRoll",
                    "savingThrowAbilityId": null,
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
            () => BattleMasterManeuverDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.battle-master-maneuver.test",
                    "name": "Test",
                    "name": "Other",
                    "effectTarget": "DamageRoll",
                    "savingThrowAbilityId": null,
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void MissingRequiredSourcesMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => BattleMasterManeuverDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.battle-master-maneuver.test",
                    "name": "Test",
                    "effectTarget": "DamageRoll",
                    "savingThrowAbilityId": null
                  }
                ]
                """));
    }

    [Fact]
    public void NullRequiredIdMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => BattleMasterManeuverDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": null,
                    "name": "Test",
                    "effectTarget": "DamageRoll",
                    "savingThrowAbilityId": null,
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
              "id": "extension.battle-master-maneuver.test",
              "name": "Test",
              "effectTarget": "DamageRoll",
              "savingThrowAbilityId": null,
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
            () => BattleMasterManeuverDefinitionLoader.LoadFromJson(json));
    }
}
