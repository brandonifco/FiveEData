using FiveEData.Rules.Spells.MagicSchools;
using FiveEData.Rules.Spells.MagicSchools.Serialization;

namespace FiveEData.Tests;

public sealed class MagicSchoolDefinitionLoaderTests
{
    [Fact]
    public void ValidDefinition_LoadsStrictly()
    {
        MagicSchoolDefinition definition = Assert.Single(
            MagicSchoolDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.magic-school.test",
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

        Assert.Equal("extension.magic-school.test", definition.Id.Value);
        Assert.Equal("Test", definition.Name);
        Assert.Single(definition.Sources);
    }

    [Fact]
    public void NullRoot_IsRejected()
    {
        Assert.ThrowsAny<Exception>(
            () => MagicSchoolDefinitionLoader.LoadFromJson("null"));
    }

    [Fact]
    public void NullElement_IsRejected()
    {
        Assert.ThrowsAny<Exception>(
            () => MagicSchoolDefinitionLoader.LoadFromJson("[null]"));
    }

    [Fact]
    public void UnknownProperty_IsRejected()
    {
        Assert.ThrowsAny<Exception>(
            () => MagicSchoolDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.magic-school.test",
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
            () => MagicSchoolDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.magic-school.test",
                    "id": "extension.magic-school.other",
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
                "id": "extension.magic-school.test",
                "name": "Test",
                "sources": [
                  { "documentId": "extension.source.test" }
                ],
                {{nulledMember}}
              }
            ]
            """;

        Assert.ThrowsAny<Exception>(
            () => MagicSchoolDefinitionLoader.LoadFromJson(json));
    }

    [Theory]
    [InlineData("\"name\": \"Test\", \"sources\": []")]
    [InlineData("\"id\": \"extension.magic-school.test\", \"sources\": []")]
    [InlineData(
        "\"id\": \"extension.magic-school.test\", \"name\": \"Test\"")]
    public void MissingRequiredMember_IsRejected(string members)
    {
        Assert.ThrowsAny<Exception>(
            () => MagicSchoolDefinitionLoader.LoadFromJson(
                $"[ {{ {members} }} ]"));
    }

    [Fact]
    public void DuplicateIds_AreRejected()
    {
        Assert.ThrowsAny<Exception>(
            () => MagicSchoolDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.magic-school.test",
                    "name": "Test",
                    "sources": [
                      { "documentId": "extension.source.test" }
                    ]
                  },
                  {
                    "id": "extension.magic-school.test",
                    "name": "Test",
                    "sources": [
                      { "documentId": "extension.source.test" }
                    ]
                  }
                ]
                """));
    }
}
