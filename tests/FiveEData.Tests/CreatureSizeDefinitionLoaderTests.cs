using FiveEData.Rules.Creatures.Sizes;
using FiveEData.Rules.Creatures.Sizes.Serialization;

namespace FiveEData.Tests;

public sealed class CreatureSizeDefinitionLoaderTests
{
    [Fact]
    public void ValidDefinition_LoadsStrictly()
    {
        CreatureSizeDefinition definition = Assert.Single(
            CreatureSizeDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.creature-size.test",
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

        Assert.Equal(
            "extension.creature-size.test",
            definition.Id.Value);
        Assert.Equal("Test", definition.Name);
        Assert.Single(definition.Sources);
    }

    [Fact]
    public void NullRootArray_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () =>
                CreatureSizeDefinitionLoader.LoadFromJson(
                    "null"));
    }

    [Fact]
    public void NullArrayElement_IsRejectedAsDataError()
    {
        InvalidDataException exception =
            Assert.Throws<InvalidDataException>(
                () =>
                    CreatureSizeDefinitionLoader.LoadFromJson(
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
                CreatureSizeDefinitionLoader.LoadFromJson(
                    """
                    [
                      {
                        "id": "extension.creature-size.test",
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
                CreatureSizeDefinitionLoader.LoadFromJson(
                    """
                    [
                      {
                        "id": "extension.creature-size.test",
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
                CreatureSizeDefinitionLoader.LoadFromJson(
                    """
                    [
                      {
                        "id": "extension.creature-size.test",
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
                CreatureSizeDefinitionLoader.LoadFromJson(
                    """
                    [
                      {
                        "id": null,
                        "name": "Test",
                        "sources": []
                      }
                    ]
                    """));
    }

    [Fact]
    public void NullRequiredNameMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () =>
                CreatureSizeDefinitionLoader.LoadFromJson(
                    """
                    [
                      {
                        "id": "extension.creature-size.test",
                        "name": null,
                        "sources": []
                      }
                    ]
                    """));
    }

    [Fact]
    public void NullRequiredSourcesMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () =>
                CreatureSizeDefinitionLoader.LoadFromJson(
                    """
                    [
                      {
                        "id": "extension.creature-size.test",
                        "name": "Test",
                        "sources": null
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
              "id": "extension.creature-size.test",
              "name": "Test",
              "sources": [
                {
                  "documentId": "extension.source.test",
                  "page": 1
                }
              ]
            }
            """;

        string json = $"[{one},{one}]";

        Assert.Throws<InvalidDataException>(
            () =>
                CreatureSizeDefinitionLoader.LoadFromJson(
                    json));
    }
}
