using FiveEData.Rules.Adventuring.Resting;
using FiveEData.Rules.Adventuring.Resting.Serialization;

namespace FiveEData.Tests;

public sealed class RestTypeDefinitionLoaderTests
{
    [Fact]
    public void ValidDefinition_LoadsStrictly()
    {
        RestTypeDefinition definition = Assert.Single(
            RestTypeDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.rest-type.test",
                    "name": "Test",
                    "minimumDurationHours": 1,
                    "cooldownHours": null,
                    "minimumHitPointsToBenefit": null,
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

        Assert.Equal("extension.rest-type.test", definition.Id.Value);
        Assert.Equal("Test", definition.Name);
        Assert.Equal(1, definition.MinimumDurationHours);
        Assert.Null(definition.CooldownHours);
        Assert.Null(definition.MinimumHitPointsToBenefit);
        Assert.Single(definition.Sources);
    }

    [Fact]
    public void NullRoot_IsRejected()
    {
        Assert.ThrowsAny<Exception>(
            () => RestTypeDefinitionLoader.LoadFromJson("null"));
    }

    [Fact]
    public void NullElement_IsRejected()
    {
        Assert.ThrowsAny<Exception>(
            () => RestTypeDefinitionLoader.LoadFromJson("[null]"));
    }

    [Fact]
    public void UnknownProperty_IsRejected()
    {
        Assert.ThrowsAny<Exception>(
            () => RestTypeDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.rest-type.test",
                    "name": "Test",
                    "minimumDurationHours": 1,
                    "cooldownHours": null,
                    "minimumHitPointsToBenefit": null,
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
            () => RestTypeDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.rest-type.test",
                    "id": "extension.rest-type.other",
                    "name": "Test",
                    "minimumDurationHours": 1,
                    "cooldownHours": null,
                    "minimumHitPointsToBenefit": null,
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
                "id": "extension.rest-type.test",
                "name": "Test",
                "minimumDurationHours": 1,
                "cooldownHours": null,
                "minimumHitPointsToBenefit": null,
                "sources": [
                  { "documentId": "extension.source.test" }
                ],
                {{nulledMember}}
              }
            ]
            """;

        Assert.ThrowsAny<Exception>(
            () => RestTypeDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void MissingRequiredMember_IsRejected()
    {
        Assert.ThrowsAny<Exception>(
            () => RestTypeDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.rest-type.test",
                    "name": "Test"
                  }
                ]
                """));
    }

    [Fact]
    public void DuplicateIds_AreRejected()
    {
        Assert.ThrowsAny<Exception>(
            () => RestTypeDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.rest-type.test",
                    "name": "Test",
                    "minimumDurationHours": 1,
                    "cooldownHours": null,
                    "minimumHitPointsToBenefit": null,
                    "sources": [
                      { "documentId": "extension.source.test" }
                    ]
                  },
                  {
                    "id": "extension.rest-type.test",
                    "name": "Test",
                    "minimumDurationHours": 1,
                    "cooldownHours": null,
                    "minimumHitPointsToBenefit": null,
                    "sources": [
                      { "documentId": "extension.source.test" }
                    ]
                  }
                ]
                """));
    }
}
