using FiveEData.Rules.Creatures.Conditions;
using FiveEData.Rules.Creatures.Conditions.Serialization;

namespace FiveEData.Tests;

public sealed class ConditionDefinitionLoaderTests
{
    [Fact]
    public void ValidDefinition_LoadsStrictly()
    {
        ConditionDefinition definition = Assert.Single(
            ConditionDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.condition.test",
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
            "extension.condition.test",
            definition.Id.Value);
        Assert.Equal("Test", definition.Name);
        Assert.Single(definition.Sources);
    }

    [Fact]
    public void NullRootArray_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () =>
                ConditionDefinitionLoader.LoadFromJson(
                    "null"));
    }

    [Fact]
    public void NullArrayElement_IsRejectedAsDataError()
    {
        InvalidDataException exception =
            Assert.Throws<InvalidDataException>(
                () =>
                    ConditionDefinitionLoader.LoadFromJson(
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
                ConditionDefinitionLoader.LoadFromJson(
                    """
                    [
                      {
                        "id": "extension.condition.test",
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
                ConditionDefinitionLoader.LoadFromJson(
                    """
                    [
                      {
                        "id": "extension.condition.test",
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
                ConditionDefinitionLoader.LoadFromJson(
                    """
                    [
                      {
                        "id": "extension.condition.test",
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
                ConditionDefinitionLoader.LoadFromJson(
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
                ConditionDefinitionLoader.LoadFromJson(
                    """
                    [
                      {
                        "id": "extension.condition.test",
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
                ConditionDefinitionLoader.LoadFromJson(
                    """
                    [
                      {
                        "id": "extension.condition.test",
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
              "id": "extension.condition.test",
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
                ConditionDefinitionLoader.LoadFromJson(
                    json));
    }
}
