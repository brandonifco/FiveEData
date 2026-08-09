using FiveEData.Rules.Combat.Cover;
using FiveEData.Rules.Combat.Cover.Serialization;

namespace FiveEData.Tests;

public sealed class CoverDefinitionLoaderTests
{
    [Fact]
    public void ValidDefinition_LoadsStrictly()
    {
        CoverDefinition definition = Assert.Single(
            CoverDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.cover.test",
                    "name": "Test",
                    "armorClassBonus": 2,
                    "dexteritySavingThrowBonus": 2,
                    "preventsBeingTargeted": false,
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

        Assert.Equal("extension.cover.test", definition.Id.Value);
        Assert.Equal("Test", definition.Name);
        Assert.Equal(2, definition.ArmorClassBonus);
        Assert.Equal(2, definition.DexteritySavingThrowBonus);
        Assert.False(definition.PreventsBeingTargeted);
        Assert.Single(definition.Sources);
    }

    [Fact]
    public void ValidDefinition_LoadsNullBonusesWithPreventsBeingTargeted()
    {
        CoverDefinition definition = Assert.Single(
            CoverDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.cover.test",
                    "name": "Test",
                    "armorClassBonus": null,
                    "dexteritySavingThrowBonus": null,
                    "preventsBeingTargeted": true,
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

        Assert.Null(definition.ArmorClassBonus);
        Assert.Null(definition.DexteritySavingThrowBonus);
        Assert.True(definition.PreventsBeingTargeted);
    }

    [Fact]
    public void NullRoot_IsRejected()
    {
        Assert.ThrowsAny<Exception>(
            () => CoverDefinitionLoader.LoadFromJson("null"));
    }

    [Fact]
    public void NullElement_IsRejected()
    {
        Assert.ThrowsAny<Exception>(
            () => CoverDefinitionLoader.LoadFromJson("[null]"));
    }

    [Fact]
    public void UnknownProperty_IsRejected()
    {
        Assert.ThrowsAny<Exception>(
            () => CoverDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.cover.test",
                    "name": "Test",
                    "armorClassBonus": 2,
                    "dexteritySavingThrowBonus": 2,
                    "preventsBeingTargeted": false,
                    "unexpected": true,
                    "sources": [
                      { "documentId": "extension.source.test" }
                    ]
                  }
                ]
                """));
    }

    [Fact]
    public void DuplicateJsonProperty_IsRejected()
    {
        Assert.ThrowsAny<Exception>(
            () => CoverDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.cover.test",
                    "id": "extension.cover.other",
                    "name": "Test",
                    "armorClassBonus": 2,
                    "dexteritySavingThrowBonus": 2,
                    "preventsBeingTargeted": false,
                    "sources": [
                      { "documentId": "extension.source.test" }
                    ]
                  }
                ]
                """));
    }

    [Theory]
    [InlineData("\"id\": null")]
    [InlineData("\"name\": null")]
    [InlineData("\"sources\": null")]
    public void NullRequiredMember_IsRejected(string nulledMember)
    {
        string json =
            $$"""
            [
              {
                "id": "extension.cover.test",
                "name": "Test",
                "armorClassBonus": 2,
                "dexteritySavingThrowBonus": 2,
                "preventsBeingTargeted": false,
                "sources": [
                  { "documentId": "extension.source.test" }
                ],
                {{nulledMember}}
              }
            ]
            """;

        Assert.ThrowsAny<Exception>(
            () => CoverDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void MissingRequiredMember_IsRejected()
    {
        Assert.ThrowsAny<Exception>(
            () => CoverDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.cover.test",
                    "name": "Test"
                  }
                ]
                """));
    }

    [Fact]
    public void DuplicateIds_AreRejected()
    {
        Assert.ThrowsAny<Exception>(
            () => CoverDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.cover.test",
                    "name": "Test",
                    "armorClassBonus": 2,
                    "dexteritySavingThrowBonus": 2,
                    "preventsBeingTargeted": false,
                    "sources": [
                      { "documentId": "extension.source.test" }
                    ]
                  },
                  {
                    "id": "extension.cover.test",
                    "name": "Test",
                    "armorClassBonus": 2,
                    "dexteritySavingThrowBonus": 2,
                    "preventsBeingTargeted": false,
                    "sources": [
                      { "documentId": "extension.source.test" }
                    ]
                  }
                ]
                """));
    }
}
