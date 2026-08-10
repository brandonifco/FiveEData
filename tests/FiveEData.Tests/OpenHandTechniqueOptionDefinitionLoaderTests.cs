using FiveEData.Rules.Classes.OpenHandTechniqueOptions;
using FiveEData.Rules.Classes.OpenHandTechniqueOptions.Serialization;
using FiveEData.Rules.Common;

namespace FiveEData.Tests;

public sealed class OpenHandTechniqueOptionDefinitionLoaderTests
{
    private const string MinimalMembers =
        """
        "savingThrowAbilityId": null,
        "imposedConditionId": null,
        "pushDistanceFeet": null,
        "preventsReactions": false,
        "preventsReactionsUntil": null
        """;

    private const string TestSources =
        """
        "sources": [
          {
            "documentId": "extension.source.test",
            "page": 1,
            "section": "Test section"
          }
        ]
        """;

    [Fact]
    public void ValidDefinition_LoadsStrictly()
    {
        OpenHandTechniqueOptionDefinition definition = Assert.Single(
            OpenHandTechniqueOptionDefinitionLoader.LoadFromJson(
                $$"""
                [
                  {
                    "id": "extension.open-hand-technique-option.test",
                    "name": "Test",
                    {{MinimalMembers}},
                    {{TestSources}}
                  }
                ]
                """));

        Assert.Equal("extension.open-hand-technique-option.test", definition.Id.Value);
        Assert.Equal("Test", definition.Name);
        Assert.Single(definition.Sources);
    }

    [Fact]
    public void ValidDefinition_LoadsMechanismFieldsWhenPresent()
    {
        OpenHandTechniqueOptionDefinition definition = Assert.Single(
            OpenHandTechniqueOptionDefinitionLoader.LoadFromJson(
                $$"""
                [
                  {
                    "id": "extension.open-hand-technique-option.test",
                    "name": "Test",
                    "savingThrowAbilityId": "dnd5e2014.ability.strength",
                    "imposedConditionId": "dnd5e2014.condition.prone",
                    "pushDistanceFeet": 15,
                    "preventsReactions": true,
                    "preventsReactionsUntil": "EndOfYourNextTurn",
                    {{TestSources}}
                  }
                ]
                """));

        Assert.Equal(
            "dnd5e2014.ability.strength",
            definition.SavingThrowAbilityId?.Value);
        Assert.Equal(
            "dnd5e2014.condition.prone",
            definition.ImposedConditionId?.Value);
        Assert.Equal(15, definition.PushDistanceFeet);
        Assert.True(definition.PreventsReactions);
        Assert.Equal(
            NextTurnDurationTrigger.EndOfYourNextTurn,
            definition.PreventsReactionsUntil);
    }

    [Fact]
    public void NullRootArray_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => OpenHandTechniqueOptionDefinitionLoader.LoadFromJson("null"));
    }

    [Fact]
    public void NullArrayElement_IsRejectedAsDataError()
    {
        InvalidDataException exception =
            Assert.Throws<InvalidDataException>(
                () => OpenHandTechniqueOptionDefinitionLoader.LoadFromJson("[null]"));

        Assert.Contains(
            "index 0",
            exception.Message,
            StringComparison.Ordinal);
    }

    [Fact]
    public void UnknownProperty_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => OpenHandTechniqueOptionDefinitionLoader.LoadFromJson(
                $$"""
                [
                  {
                    "id": "extension.open-hand-technique-option.test",
                    "name": "Test",
                    {{MinimalMembers}},
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
            () => OpenHandTechniqueOptionDefinitionLoader.LoadFromJson(
                $$"""
                [
                  {
                    "id": "extension.open-hand-technique-option.test",
                    "name": "Test",
                    "name": "Other",
                    {{MinimalMembers}},
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void MissingRequiredSourcesMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => OpenHandTechniqueOptionDefinitionLoader.LoadFromJson(
                $$"""
                [
                  {
                    "id": "extension.open-hand-technique-option.test",
                    "name": "Test",
                    {{MinimalMembers}}
                  }
                ]
                """));
    }

    [Fact]
    public void NullRequiredIdMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => OpenHandTechniqueOptionDefinitionLoader.LoadFromJson(
                $$"""
                [
                  {
                    "id": null,
                    "name": "Test",
                    {{MinimalMembers}},
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void DuplicateIds_AreRejected()
    {
        string one =
            $$"""
            {
              "id": "extension.open-hand-technique-option.test",
              "name": "Test",
              {{MinimalMembers}},
              {{TestSources}}
            }
            """;

        Assert.Throws<InvalidDataException>(
            () => OpenHandTechniqueOptionDefinitionLoader.LoadFromJson($"[{one},{one}]"));
    }
}
