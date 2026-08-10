using FiveEData.Rules.Classes.ChannelDivinityOptions;
using FiveEData.Rules.Classes.ChannelDivinityOptions.Serialization;
using FiveEData.Rules.Common;

namespace FiveEData.Tests;

public sealed class ChannelDivinityOptionDefinitionLoaderTests
{
    [Fact]
    public void ValidDefinition_LoadsWithAllFactsNull()
    {
        ChannelDivinityOptionDefinition definition = Assert.Single(
            ChannelDivinityOptionDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.channel-divinity-option.test",
                    "name": "Test",
                    "rangeFeet": null,
                    "savingThrowAbilityId": null,
                    "durationMinutes": null,
                    "rollBonus": null,
                    "imposedConditionId": null,
                    "conditionDurationTrigger": null,
                    "maximizesDamageRoll": false,
                    "grantedSpellId": null,
                    "automaticallyFailsGrantedSpellSave": false,
                    "addsSpellcastingModifierToAttackRolls": false,
                    "brightLightRadiusFeet": null,
                    "dimLightRadiusFeet": null,
                    "choosableSavingThrowAbilityIds": [],
                    "grantsAdvantageOnAttackRollsAgainstTarget": false,
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
            "extension.channel-divinity-option.test",
            definition.Id.Value);
        Assert.Equal("Test", definition.Name);
        Assert.Null(definition.RangeFeet);
        Assert.Null(definition.SavingThrowAbilityId);
        Assert.Null(definition.DurationMinutes);
        Assert.Null(definition.RollBonus);
        Assert.Empty(definition.ChoosableSavingThrowAbilityIds);
        Assert.Single(definition.Sources);
    }

    [Fact]
    public void ValidDefinition_LoadsAllFactsWhenPresent()
    {
        ChannelDivinityOptionDefinition definition = Assert.Single(
            ChannelDivinityOptionDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.channel-divinity-option.test",
                    "name": "Test",
                    "rangeFeet": 60,
                    "savingThrowAbilityId": "dnd5e2014.ability.wisdom",
                    "durationMinutes": 1,
                    "rollBonus": 10,
                    "imposedConditionId": null,
                    "conditionDurationTrigger": null,
                    "maximizesDamageRoll": false,
                    "grantedSpellId": null,
                    "automaticallyFailsGrantedSpellSave": false,
                    "addsSpellcastingModifierToAttackRolls": false,
                    "brightLightRadiusFeet": null,
                    "dimLightRadiusFeet": null,
                    "choosableSavingThrowAbilityIds": [],
                    "grantsAdvantageOnAttackRollsAgainstTarget": false,
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

        Assert.Equal(60, definition.RangeFeet);
        Assert.Equal(
            "dnd5e2014.ability.wisdom",
            definition.SavingThrowAbilityId?.Value);
        Assert.Equal(1, definition.DurationMinutes);
        Assert.Equal(10, definition.RollBonus);
    }

    [Fact]
    public void ValidDefinition_LoadsMechanismFieldsWhenPresent()
    {
        ChannelDivinityOptionDefinition definition = Assert.Single(
            ChannelDivinityOptionDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.channel-divinity-option.test",
                    "name": "Test",
                    "rangeFeet": null,
                    "savingThrowAbilityId": null,
                    "durationMinutes": null,
                    "rollBonus": null,
                    "imposedConditionId": "dnd5e2014.condition.invisible",
                    "conditionDurationTrigger": "EndOfYourNextTurn",
                    "maximizesDamageRoll": true,
                    "grantedSpellId": "dnd5e2014.spell.suggestion",
                    "automaticallyFailsGrantedSpellSave": true,
                    "sources": [
                      {
                        "documentId": "extension.source.test",
                        "page": 1,
                        "section": "Test section"
                      }
                    ],
                    "addsSpellcastingModifierToAttackRolls": true,
                    "brightLightRadiusFeet": 20,
                    "dimLightRadiusFeet": 20,
                    "choosableSavingThrowAbilityIds": [
                      "dnd5e2014.ability.strength",
                      "dnd5e2014.ability.dexterity"
                    ],
                    "grantsAdvantageOnAttackRollsAgainstTarget": true
                  }
                ]
                """));

        Assert.Equal(
            "dnd5e2014.condition.invisible",
            definition.ImposedConditionId?.Value);
        Assert.Equal(
            NextTurnDurationTrigger.EndOfYourNextTurn,
            definition.ConditionDurationTrigger);
        Assert.True(definition.MaximizesDamageRoll);
        Assert.Equal(
            "dnd5e2014.spell.suggestion",
            definition.GrantedSpellId?.Value);
        Assert.True(definition.AutomaticallyFailsGrantedSpellSave);
        Assert.True(definition.AddsSpellcastingModifierToAttackRolls);
        Assert.Equal(20, definition.BrightLightRadiusFeet);
        Assert.Equal(20, definition.DimLightRadiusFeet);
        Assert.Equal(
            [
                "dnd5e2014.ability.strength",
                "dnd5e2014.ability.dexterity"
            ],
            definition.ChoosableSavingThrowAbilityIds
                .Select(abilityId => abilityId.Value)
                .ToArray());
        Assert.True(definition.GrantsAdvantageOnAttackRollsAgainstTarget);
    }

    [Fact]
    public void NullRootArray_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => ChannelDivinityOptionDefinitionLoader.LoadFromJson(
                "null"));
    }

    [Fact]
    public void NullArrayElement_IsRejectedAsDataError()
    {
        InvalidDataException exception =
            Assert.Throws<InvalidDataException>(
                () => ChannelDivinityOptionDefinitionLoader.LoadFromJson(
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
            () => ChannelDivinityOptionDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.channel-divinity-option.test",
                    "name": "Test",
                    "rangeFeet": null,
                    "savingThrowAbilityId": null,
                    "durationMinutes": null,
                    "rollBonus": null,
                    "imposedConditionId": null,
                    "conditionDurationTrigger": null,
                    "maximizesDamageRoll": false,
                    "grantedSpellId": null,
                    "automaticallyFailsGrantedSpellSave": false,
                    "addsSpellcastingModifierToAttackRolls": false,
                    "brightLightRadiusFeet": null,
                    "dimLightRadiusFeet": null,
                    "choosableSavingThrowAbilityIds": [],
                    "grantsAdvantageOnAttackRollsAgainstTarget": false,
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
            () => ChannelDivinityOptionDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.channel-divinity-option.test",
                    "name": "Test",
                    "name": "Other",
                    "rangeFeet": null,
                    "savingThrowAbilityId": null,
                    "durationMinutes": null,
                    "rollBonus": null,
                    "imposedConditionId": null,
                    "conditionDurationTrigger": null,
                    "maximizesDamageRoll": false,
                    "grantedSpellId": null,
                    "automaticallyFailsGrantedSpellSave": false,
                    "addsSpellcastingModifierToAttackRolls": false,
                    "brightLightRadiusFeet": null,
                    "dimLightRadiusFeet": null,
                    "choosableSavingThrowAbilityIds": [],
                    "grantsAdvantageOnAttackRollsAgainstTarget": false,
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void MissingRequiredSourcesMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => ChannelDivinityOptionDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.channel-divinity-option.test",
                    "name": "Test",
                    "rangeFeet": null,
                    "savingThrowAbilityId": null,
                    "durationMinutes": null,
                    "rollBonus": null,
                    "imposedConditionId": null,
                    "conditionDurationTrigger": null,
                    "maximizesDamageRoll": false,
                    "grantedSpellId": null,
                    "automaticallyFailsGrantedSpellSave": false,
                    "addsSpellcastingModifierToAttackRolls": false,
                    "brightLightRadiusFeet": null,
                    "dimLightRadiusFeet": null,
                    "choosableSavingThrowAbilityIds": [],
                    "grantsAdvantageOnAttackRollsAgainstTarget": false
                  }
                ]
                """));
    }

    [Fact]
    public void NullRequiredIdMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => ChannelDivinityOptionDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": null,
                    "name": "Test",
                    "rangeFeet": null,
                    "savingThrowAbilityId": null,
                    "durationMinutes": null,
                    "rollBonus": null,
                    "imposedConditionId": null,
                    "conditionDurationTrigger": null,
                    "maximizesDamageRoll": false,
                    "grantedSpellId": null,
                    "automaticallyFailsGrantedSpellSave": false,
                    "addsSpellcastingModifierToAttackRolls": false,
                    "brightLightRadiusFeet": null,
                    "dimLightRadiusFeet": null,
                    "choosableSavingThrowAbilityIds": [],
                    "grantsAdvantageOnAttackRollsAgainstTarget": false,
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
              "id": "extension.channel-divinity-option.test",
              "name": "Test",
              "rangeFeet": null,
              "savingThrowAbilityId": null,
              "durationMinutes": null,
              "rollBonus": null,
              "imposedConditionId": null,
              "conditionDurationTrigger": null,
              "maximizesDamageRoll": false,
              "grantedSpellId": null,
              "automaticallyFailsGrantedSpellSave": false,
              "addsSpellcastingModifierToAttackRolls": false,
              "brightLightRadiusFeet": null,
              "dimLightRadiusFeet": null,
              "choosableSavingThrowAbilityIds": [],
              "grantsAdvantageOnAttackRollsAgainstTarget": false,
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
            () => ChannelDivinityOptionDefinitionLoader.LoadFromJson(json));
    }
}
