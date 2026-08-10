using FiveEData.Rules.Classes.BattleMasterManeuvers;
using FiveEData.Rules.Classes.BattleMasterManeuvers.Serialization;
using FiveEData.Rules.Common;

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
                    "imposedConditionId": null,
                    "maximumTargetSizeId": null,
                    "pushDistanceFeet": null,
                    "reachIncreaseFeet": null,
                    "secondaryTargetRangeFeet": null,
                    "forcesDroppedItem": false,
                    "grantsAdvantageOnNextAttackRoll": false,
                    "grantsAdvantageToNextAttackAgainstTarget": false,
                    "imposesDisadvantageOnAttacksAgainstOthers": false,
                    "allowsAllyReactionMovement": false,
                    "secondaryEffectDurationTrigger": null,
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
                    "imposedConditionId": null,
                    "maximumTargetSizeId": null,
                    "pushDistanceFeet": null,
                    "reachIncreaseFeet": null,
                    "secondaryTargetRangeFeet": null,
                    "forcesDroppedItem": false,
                    "grantsAdvantageOnNextAttackRoll": false,
                    "grantsAdvantageToNextAttackAgainstTarget": false,
                    "imposesDisadvantageOnAttacksAgainstOthers": false,
                    "allowsAllyReactionMovement": false,
                    "secondaryEffectDurationTrigger": null,
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
    public void ValidDefinition_LoadsMechanismFieldsWhenPresent()
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
                    "imposedConditionId": "dnd5e2014.condition.prone",
                    "maximumTargetSizeId": "dnd5e2014.creature-size.large",
                    "pushDistanceFeet": 15,
                    "reachIncreaseFeet": 5,
                    "secondaryTargetRangeFeet": 5,
                    "forcesDroppedItem": true,
                    "grantsAdvantageOnNextAttackRoll": true,
                    "grantsAdvantageToNextAttackAgainstTarget": true,
                    "imposesDisadvantageOnAttacksAgainstOthers": true,
                    "allowsAllyReactionMovement": true,
                    "secondaryEffectDurationTrigger": "EndOfYourNextTurn",
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
            "dnd5e2014.condition.prone",
            definition.ImposedConditionId?.Value);
        Assert.Equal(
            "dnd5e2014.creature-size.large",
            definition.MaximumTargetSizeId?.Value);
        Assert.Equal(15, definition.PushDistanceFeet);
        Assert.Equal(5, definition.ReachIncreaseFeet);
        Assert.Equal(5, definition.SecondaryTargetRangeFeet);
        Assert.True(definition.ForcesDroppedItem);
        Assert.True(definition.GrantsAdvantageOnNextAttackRoll);
        Assert.True(definition.GrantsAdvantageToNextAttackAgainstTarget);
        Assert.True(definition.ImposesDisadvantageOnAttacksAgainstOthers);
        Assert.True(definition.AllowsAllyReactionMovement);
        Assert.Equal(
            NextTurnDurationTrigger.EndOfYourNextTurn,
            definition.SecondaryEffectDurationTrigger);
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
                    "imposedConditionId": null,
                    "maximumTargetSizeId": null,
                    "pushDistanceFeet": null,
                    "reachIncreaseFeet": null,
                    "secondaryTargetRangeFeet": null,
                    "forcesDroppedItem": false,
                    "grantsAdvantageOnNextAttackRoll": false,
                    "grantsAdvantageToNextAttackAgainstTarget": false,
                    "imposesDisadvantageOnAttacksAgainstOthers": false,
                    "allowsAllyReactionMovement": false,
                    "secondaryEffectDurationTrigger": null,
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
                    "imposedConditionId": null,
                    "maximumTargetSizeId": null,
                    "pushDistanceFeet": null,
                    "reachIncreaseFeet": null,
                    "secondaryTargetRangeFeet": null,
                    "forcesDroppedItem": false,
                    "grantsAdvantageOnNextAttackRoll": false,
                    "grantsAdvantageToNextAttackAgainstTarget": false,
                    "imposesDisadvantageOnAttacksAgainstOthers": false,
                    "allowsAllyReactionMovement": false,
                    "secondaryEffectDurationTrigger": null,
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
                    "savingThrowAbilityId": null,
                    "imposedConditionId": null,
                    "maximumTargetSizeId": null,
                    "pushDistanceFeet": null,
                    "reachIncreaseFeet": null,
                    "secondaryTargetRangeFeet": null,
                    "forcesDroppedItem": false,
                    "grantsAdvantageOnNextAttackRoll": false,
                    "grantsAdvantageToNextAttackAgainstTarget": false,
                    "imposesDisadvantageOnAttacksAgainstOthers": false,
                    "allowsAllyReactionMovement": false,
                    "secondaryEffectDurationTrigger": null
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
                    "imposedConditionId": null,
                    "maximumTargetSizeId": null,
                    "pushDistanceFeet": null,
                    "reachIncreaseFeet": null,
                    "secondaryTargetRangeFeet": null,
                    "forcesDroppedItem": false,
                    "grantsAdvantageOnNextAttackRoll": false,
                    "grantsAdvantageToNextAttackAgainstTarget": false,
                    "imposesDisadvantageOnAttacksAgainstOthers": false,
                    "allowsAllyReactionMovement": false,
                    "secondaryEffectDurationTrigger": null,
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
              "imposedConditionId": null,
              "maximumTargetSizeId": null,
              "pushDistanceFeet": null,
              "reachIncreaseFeet": null,
              "secondaryTargetRangeFeet": null,
              "forcesDroppedItem": false,
              "grantsAdvantageOnNextAttackRoll": false,
              "grantsAdvantageToNextAttackAgainstTarget": false,
              "imposesDisadvantageOnAttacksAgainstOthers": false,
              "allowsAllyReactionMovement": false,
              "secondaryEffectDurationTrigger": null,
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
