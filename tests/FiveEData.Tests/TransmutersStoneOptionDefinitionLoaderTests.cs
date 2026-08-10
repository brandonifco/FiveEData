using FiveEData.Rules.Classes.TransmutersStoneOptions;
using FiveEData.Rules.Classes.TransmutersStoneOptions.Serialization;

namespace FiveEData.Tests;

public sealed class TransmutersStoneOptionDefinitionLoaderTests
{
    private const string MinimalMembers =
        """
        "darkvisionRangeFeet": null,
        "speedBonusFeet": null,
        "requiresUnencumbered": false,
        "savingThrowProficiencyAbilityId": null,
        "choosableResistedDamageTypeIds": []
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
        TransmutersStoneOptionDefinition definition = Assert.Single(
            TransmutersStoneOptionDefinitionLoader.LoadFromJson(
                $$"""
                [
                  {
                    "id": "extension.transmuters-stone-option.test",
                    "name": "Test",
                    {{MinimalMembers}},
                    {{TestSources}}
                  }
                ]
                """));

        Assert.Equal("extension.transmuters-stone-option.test", definition.Id.Value);
        Assert.Equal("Test", definition.Name);
        Assert.Single(definition.Sources);
    }

    [Fact]
    public void ValidDefinition_LoadsMechanismFieldsWhenPresent()
    {
        TransmutersStoneOptionDefinition definition = Assert.Single(
            TransmutersStoneOptionDefinitionLoader.LoadFromJson(
                $$"""
                [
                  {
                    "id": "extension.transmuters-stone-option.test",
                    "name": "Test",
                    "darkvisionRangeFeet": 60,
                    "speedBonusFeet": 10,
                    "requiresUnencumbered": true,
                    "savingThrowProficiencyAbilityId":
                      "dnd5e2014.ability.constitution",
                    "choosableResistedDamageTypeIds": [
                      "dnd5e2014.damage-type.acid",
                      "dnd5e2014.damage-type.cold"
                    ],
                    {{TestSources}}
                  }
                ]
                """));

        Assert.Equal(60, definition.DarkvisionRangeFeet);
        Assert.Equal(10, definition.SpeedBonusFeet);
        Assert.True(definition.RequiresUnencumbered);
        Assert.Equal(
            "dnd5e2014.ability.constitution",
            definition.SavingThrowProficiencyAbilityId?.Value);
        Assert.Equal(2, definition.ChoosableResistedDamageTypeIds.Count);
    }

    [Fact]
    public void NullRootArray_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => TransmutersStoneOptionDefinitionLoader.LoadFromJson("null"));
    }

    [Fact]
    public void NullArrayElement_IsRejectedAsDataError()
    {
        InvalidDataException exception =
            Assert.Throws<InvalidDataException>(
                () => TransmutersStoneOptionDefinitionLoader.LoadFromJson("[null]"));

        Assert.Contains(
            "index 0",
            exception.Message,
            StringComparison.Ordinal);
    }

    [Fact]
    public void UnknownProperty_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => TransmutersStoneOptionDefinitionLoader.LoadFromJson(
                $$"""
                [
                  {
                    "id": "extension.transmuters-stone-option.test",
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
            () => TransmutersStoneOptionDefinitionLoader.LoadFromJson(
                $$"""
                [
                  {
                    "id": "extension.transmuters-stone-option.test",
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
            () => TransmutersStoneOptionDefinitionLoader.LoadFromJson(
                $$"""
                [
                  {
                    "id": "extension.transmuters-stone-option.test",
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
            () => TransmutersStoneOptionDefinitionLoader.LoadFromJson(
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
              "id": "extension.transmuters-stone-option.test",
              "name": "Test",
              {{MinimalMembers}},
              {{TestSources}}
            }
            """;

        Assert.Throws<InvalidDataException>(
            () => TransmutersStoneOptionDefinitionLoader.LoadFromJson($"[{one},{one}]"));
    }
}
