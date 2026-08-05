using FiveEData.Rules.Creatures.Alignments;
using FiveEData.Rules.Creatures.Alignments.Serialization;

namespace FiveEData.Tests;

public sealed class AlignmentDefinitionLoaderTests
{
    [Fact]
    public void ValidDefinition_LoadsStrictly()
    {
        AlignmentDefinition definition = Assert.Single(
            AlignmentDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.alignment.test",
                    "name": "Test",
                    "ethic": "Neutral",
                    "morality": "Neutral",
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
            "extension.alignment.test",
            definition.Id.Value);
        Assert.Equal("Test", definition.Name);
        Assert.Equal(AlignmentEthic.Neutral, definition.Ethic);
        Assert.Equal(
            AlignmentMorality.Neutral,
            definition.Morality);
        Assert.Single(definition.Sources);
    }

    [Fact]
    public void NullRootArray_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () =>
                AlignmentDefinitionLoader.LoadFromJson(
                    "null"));
    }

    [Fact]
    public void NullArrayElement_IsRejectedAsDataError()
    {
        InvalidDataException exception =
            Assert.Throws<InvalidDataException>(
                () =>
                    AlignmentDefinitionLoader.LoadFromJson(
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
                AlignmentDefinitionLoader.LoadFromJson(
                    """
                    [
                      {
                        "id": "extension.alignment.test",
                        "name": "Test",
                        "ethic": "Neutral",
                        "morality": "Neutral",
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
                AlignmentDefinitionLoader.LoadFromJson(
                    """
                    [
                      {
                        "id": "extension.alignment.test",
                        "name": "Test",
                        "name": "Other",
                        "ethic": "Neutral",
                        "morality": "Neutral",
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
                AlignmentDefinitionLoader.LoadFromJson(
                    """
                    [
                      {
                        "id": "extension.alignment.test",
                        "name": "Test",
                        "ethic": "Neutral",
                        "morality": "Neutral"
                      }
                    ]
                    """));
    }

    [Fact]
    public void MissingRequiredEthicMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () =>
                AlignmentDefinitionLoader.LoadFromJson(
                    """
                    [
                      {
                        "id": "extension.alignment.test",
                        "name": "Test",
                        "morality": "Neutral",
                        "sources": []
                      }
                    ]
                    """));
    }

    [Fact]
    public void MissingRequiredMoralityMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () =>
                AlignmentDefinitionLoader.LoadFromJson(
                    """
                    [
                      {
                        "id": "extension.alignment.test",
                        "name": "Test",
                        "ethic": "Neutral",
                        "sources": []
                      }
                    ]
                    """));
    }

    [Fact]
    public void NullRequiredIdMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () =>
                AlignmentDefinitionLoader.LoadFromJson(
                    """
                    [
                      {
                        "id": null,
                        "name": "Test",
                        "ethic": "Neutral",
                        "morality": "Neutral",
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
                AlignmentDefinitionLoader.LoadFromJson(
                    """
                    [
                      {
                        "id": "extension.alignment.test",
                        "name": null,
                        "ethic": "Neutral",
                        "morality": "Neutral",
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
                AlignmentDefinitionLoader.LoadFromJson(
                    """
                    [
                      {
                        "id": "extension.alignment.test",
                        "name": "Test",
                        "ethic": "Neutral",
                        "morality": "Neutral",
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
              "id": "extension.alignment.test",
              "name": "Test",
              "ethic": "Neutral",
              "morality": "Neutral",
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
                AlignmentDefinitionLoader.LoadFromJson(
                    json));
    }
}
