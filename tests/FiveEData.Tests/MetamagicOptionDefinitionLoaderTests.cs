using FiveEData.Rules.Classes.Metamagic;
using FiveEData.Rules.Classes.Metamagic.Serialization;

namespace FiveEData.Tests;

public sealed class MetamagicOptionDefinitionLoaderTests
{
    [Fact]
    public void ValidDefinition_LoadsFixedCostStrictly()
    {
        MetamagicOptionDefinition definition = Assert.Single(
            MetamagicOptionDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.metamagic-option.test",
                    "name": "Test",
                    "fixedSorceryPointCost": 1,
                    "costEqualsSpellLevelWithCantripMinimum": false,
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
            "extension.metamagic-option.test",
            definition.Id.Value);
        Assert.Equal("Test", definition.Name);
        Assert.Equal(1, definition.FixedSorceryPointCost);
        Assert.False(definition.CostEqualsSpellLevelWithCantripMinimum);
        Assert.Single(definition.Sources);
    }

    [Fact]
    public void ValidDefinition_LoadsSpellLevelCostRepresentation()
    {
        MetamagicOptionDefinition definition = Assert.Single(
            MetamagicOptionDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.metamagic-option.test",
                    "name": "Test",
                    "fixedSorceryPointCost": null,
                    "costEqualsSpellLevelWithCantripMinimum": true,
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

        Assert.Null(definition.FixedSorceryPointCost);
        Assert.True(definition.CostEqualsSpellLevelWithCantripMinimum);
    }

    [Fact]
    public void NullRootArray_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => MetamagicOptionDefinitionLoader.LoadFromJson("null"));
    }

    [Fact]
    public void NullArrayElement_IsRejectedAsDataError()
    {
        InvalidDataException exception =
            Assert.Throws<InvalidDataException>(
                () => MetamagicOptionDefinitionLoader.LoadFromJson(
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
            () => MetamagicOptionDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.metamagic-option.test",
                    "name": "Test",
                    "fixedSorceryPointCost": 1,
                    "costEqualsSpellLevelWithCantripMinimum": false,
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
            () => MetamagicOptionDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.metamagic-option.test",
                    "name": "Test",
                    "name": "Other",
                    "fixedSorceryPointCost": 1,
                    "costEqualsSpellLevelWithCantripMinimum": false,
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void MissingRequiredSourcesMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => MetamagicOptionDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.metamagic-option.test",
                    "name": "Test",
                    "fixedSorceryPointCost": 1,
                    "costEqualsSpellLevelWithCantripMinimum": false
                  }
                ]
                """));
    }

    [Fact]
    public void NullRequiredIdMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => MetamagicOptionDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": null,
                    "name": "Test",
                    "fixedSorceryPointCost": 1,
                    "costEqualsSpellLevelWithCantripMinimum": false,
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
              "id": "extension.metamagic-option.test",
              "name": "Test",
              "fixedSorceryPointCost": 1,
              "costEqualsSpellLevelWithCantripMinimum": false,
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
            () => MetamagicOptionDefinitionLoader.LoadFromJson(json));
    }
}
