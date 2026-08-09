using FiveEData.Rules.Creatures.Conditions;
using FiveEData.Rules.Creatures.Conditions.Serialization;

namespace FiveEData.Tests;

public sealed class ConditionDefinitionLoaderTests
{
    private const string ValidCondition =
        """
        {
          "id": "extension.condition.test",
          "name": "Test",
          "preventsActionsAndReactions": false,
          "preventsMovement": false,
          "onlyMovementOptionIsToCrawl": false,
          "speedBecomesZero": false,
          "ignoresBonusesToSpeed": false,
          "speechRestriction": "None",
          "unawareOfSurroundings": false,
          "automaticallyFailsStrengthAndDexteritySavingThrows": false,
          "dexteritySavingThrowsHaveDisadvantage": false,
          "automaticallyFailsAbilityChecksRequiringSight": false,
          "automaticallyFailsAbilityChecksRequiringHearing": false,
          "ownAbilityChecksHaveDisadvantage": false,
          "attackRollsAgainstTheCreature": "None",
          "theCreaturesOwnAttackRolls": "None",
          "anyHitIsACriticalHitIfAttackerIsWithinFiveFeet": false,
          "requiresSourceInLineOfSightForRollEffects": false,
          "cannotWillinglyMoveCloserToSource": false,
          "cannotAttackOrTargetSourceWithHarmfulEffects": false,
          "sourceHasAdvantageOnSocialAbilityChecksAgainstTheCreature": false,
          "endsIfSourceCreatureIsIncapacitated": false,
          "resistantToAllDamage": false,
          "immuneToPoisonAndDisease": false,
          "weightMultiplier": null,
          "dropsHeldItemsAndFallsProne": false,
          "heavilyObscuredForHidingPurposes": false,
          "exhaustionEffect": null,
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
        ConditionDefinition definition = Assert.Single(
            ConditionDefinitionLoader.LoadFromJson($"[{ValidCondition}]"));

        Assert.Equal(
            "extension.condition.test",
            definition.Id.Value);
        Assert.Equal("Test", definition.Name);
        Assert.False(definition.PreventsActionsAndReactions);
        Assert.Null(definition.WeightMultiplier);
        Assert.Null(definition.ExhaustionEffect);
        Assert.Single(definition.Sources);
    }

    [Fact]
    public void ValidDefinition_LoadsExhaustionEffect()
    {
        ConditionDefinition definition = Assert.Single(
            ConditionDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.condition.exhaustion-test",
                    "name": "Test",
                    "preventsActionsAndReactions": false,
                    "preventsMovement": false,
                    "onlyMovementOptionIsToCrawl": false,
                    "speedBecomesZero": false,
                    "ignoresBonusesToSpeed": false,
                    "speechRestriction": "None",
                    "unawareOfSurroundings": false,
                    "automaticallyFailsStrengthAndDexteritySavingThrows": false,
                    "dexteritySavingThrowsHaveDisadvantage": false,
                    "automaticallyFailsAbilityChecksRequiringSight": false,
                    "automaticallyFailsAbilityChecksRequiringHearing": false,
                    "ownAbilityChecksHaveDisadvantage": false,
                    "attackRollsAgainstTheCreature": "None",
                    "theCreaturesOwnAttackRolls": "None",
                    "anyHitIsACriticalHitIfAttackerIsWithinFiveFeet": false,
                    "requiresSourceInLineOfSightForRollEffects": false,
                    "cannotWillinglyMoveCloserToSource": false,
                    "cannotAttackOrTargetSourceWithHarmfulEffects": false,
                    "sourceHasAdvantageOnSocialAbilityChecksAgainstTheCreature": false,
                    "endsIfSourceCreatureIsIncapacitated": false,
                    "resistantToAllDamage": false,
                    "immuneToPoisonAndDisease": false,
                    "weightMultiplier": null,
                    "dropsHeldItemsAndFallsProne": false,
                    "heavilyObscuredForHidingPurposes": false,
                    "exhaustionEffect": {
                      "levelEffects": [
                        "DisadvantageOnAbilityChecks",
                        "SpeedHalved",
                        "DisadvantageOnAttackRollsAndSavingThrows",
                        "HitPointMaximumHalved",
                        "SpeedReducedToZero",
                        "Death"
                      ],
                      "recoversOneLevelPerLongRest": true,
                      "recoveryRequiresFoodAndDrink": true
                    },
                    "sources": [
                      { "documentId": "extension.source.test", "page": 1, "section": "Test section" }
                    ]
                  }
                ]
                """));

        ExhaustionEffectDetail exhaustionEffect =
            definition.ExhaustionEffect
            ?? throw new InvalidOperationException(
                "Expected an exhaustion effect.");

        Assert.Equal(6, exhaustionEffect.LevelEffects.Count);
        Assert.True(exhaustionEffect.RecoversOneLevelPerLongRest);
        Assert.True(exhaustionEffect.RecoveryRequiresFoodAndDrink);
    }

    [Fact]
    public void NullRootArray_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () =>
                ConditionDefinitionLoader.LoadFromJson(
                    "null"));
    }

    [Fact]
    public void NullArrayElement_IsRejectedAsDataError()
    {
        InvalidDataException exception =
            Assert.Throws<InvalidDataException>(
                () =>
                    ConditionDefinitionLoader.LoadFromJson(
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
            () =>
                ConditionDefinitionLoader.LoadFromJson(
                    """
                    [
                      {
                        "id": "extension.condition.test",
                        "name": "Test",
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
            () =>
                ConditionDefinitionLoader.LoadFromJson(
                    """
                    [
                      {
                        "id": "extension.condition.test",
                        "name": "Test",
                        "name": "Other",
                        "sources": []
                      }
                    ]
                    """));
    }

    [Fact]
    public void MissingRequiredSourcesMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () =>
                ConditionDefinitionLoader.LoadFromJson(
                    """
                    [
                      {
                        "id": "extension.condition.test",
                        "name": "Test"
                      }
                    ]
                    """));
    }

    [Fact]
    public void NullRequiredIdMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () =>
                ConditionDefinitionLoader.LoadFromJson(
                    ReplaceField(
                        "\"id\": \"extension.condition.test\"",
                        "\"id\": null")));
    }

    [Fact]
    public void NullRequiredNameMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () =>
                ConditionDefinitionLoader.LoadFromJson(
                    ReplaceField(
                        "\"name\": \"Test\"",
                        "\"name\": null")));
    }

    [Fact]
    public void NullRequiredSourcesMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () =>
                ConditionDefinitionLoader.LoadFromJson(
                    """
                    [
                      {
                        "id": "extension.condition.test",
                        "name": "Test",
                        "sources": null
                      }
                    ]
                    """));
    }

    [Fact]
    public void NullRequiredWeightMultiplierMember_IsRejected()
    {
        // weightMultiplier is nullable, so a missing key (not a JSON
        // null) is what StrictJson's [JsonRequired] rejects.
        Assert.Throws<InvalidDataException>(
            () =>
                ConditionDefinitionLoader.LoadFromJson(
                    """
                    [
                      {
                        "id": "extension.condition.test",
                        "name": "Test",
                        "sources": []
                      }
                    ]
                    """));
    }

    [Fact]
    public void DuplicateIds_AreRejected()
    {
        string json = $"[{ValidCondition},{ValidCondition}]";

        Assert.Throws<InvalidDataException>(
            () =>
                ConditionDefinitionLoader.LoadFromJson(
                    json));
    }

    private static string ReplaceField(string original, string replacement)
    {
        return $"[{ValidCondition.Replace(original, replacement)}]";
    }
}
