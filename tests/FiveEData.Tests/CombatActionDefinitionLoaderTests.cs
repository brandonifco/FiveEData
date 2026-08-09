using FiveEData.Rules.Combat.CombatActions;
using FiveEData.Rules.Combat.CombatActions.Serialization;

namespace FiveEData.Tests;

public sealed class CombatActionDefinitionLoaderTests
{
    [Fact]
    public void ValidDefinition_LoadsStrictly()
    {
        CombatActionDefinition definition = Assert.Single(
            CombatActionDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.combat-action.test",
                    "name": "Test",
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

        Assert.Equal("extension.combat-action.test", definition.Id.Value);
        Assert.Equal("Test", definition.Name);
        Assert.Single(definition.Sources);
    }

    [Fact]
    public void NullRoot_IsRejected()
    {
        Assert.ThrowsAny<Exception>(
            () => CombatActionDefinitionLoader.LoadFromJson("null"));
    }

    [Fact]
    public void NullElement_IsRejected()
    {
        Assert.ThrowsAny<Exception>(
            () => CombatActionDefinitionLoader.LoadFromJson("[null]"));
    }

    [Fact]
    public void UnknownProperty_IsRejected()
    {
        Assert.ThrowsAny<Exception>(
            () => CombatActionDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.combat-action.test",
                    "name": "Test",
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
            () => CombatActionDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.combat-action.test",
                    "id": "extension.combat-action.other",
                    "name": "Test",
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
                "id": "extension.combat-action.test",
                "name": "Test",
                "sources": [
                  { "documentId": "extension.source.test" }
                ],
                {{nulledMember}}
              }
            ]
            """;

        Assert.ThrowsAny<Exception>(
            () => CombatActionDefinitionLoader.LoadFromJson(json));
    }

    [Theory]
    [InlineData("\"name\": \"Test\", \"sources\": []")]
    [InlineData("\"id\": \"extension.combat-action.test\", \"sources\": []")]
    [InlineData(
        "\"id\": \"extension.combat-action.test\", \"name\": \"Test\"")]
    public void MissingRequiredMember_IsRejected(string members)
    {
        Assert.ThrowsAny<Exception>(
            () => CombatActionDefinitionLoader.LoadFromJson(
                $"[ {{ {members} }} ]"));
    }

    [Fact]
    public void DuplicateIds_AreRejected()
    {
        Assert.ThrowsAny<Exception>(
            () => CombatActionDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.combat-action.test",
                    "name": "Test",
                    "sources": [
                      { "documentId": "extension.source.test" }
                    ]
                  },
                  {
                    "id": "extension.combat-action.test",
                    "name": "Test",
                    "sources": [
                      { "documentId": "extension.source.test" }
                    ]
                  }
                ]
                """));
    }
}
