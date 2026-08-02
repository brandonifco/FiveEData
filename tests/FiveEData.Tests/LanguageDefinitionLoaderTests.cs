using FiveEData.Rules.Creatures.Languages;
using FiveEData.Rules.Creatures.Languages.Serialization;

namespace FiveEData.Tests;

public sealed class LanguageDefinitionLoaderTests
{
    [Fact]
    public void ValidDefinition_LoadsStrictly()
    {
        LanguageDefinition definition = Assert.Single(
            LanguageDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.language.test",
                    "name": "Test",
                    "category": "Standard",
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
            "extension.language.test",
            definition.Id.Value);
        Assert.Equal("Test", definition.Name);
        Assert.Equal(
            LanguageCategory.Standard,
            definition.Category);
        Assert.Single(definition.Sources);
    }

    [Fact]
    public void NullRootArray_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () =>
                LanguageDefinitionLoader.LoadFromJson(
                    "null"));
    }

    [Fact]
    public void NullArrayElement_IsRejectedAsDataError()
    {
        InvalidDataException exception =
            Assert.Throws<InvalidDataException>(
                () =>
                    LanguageDefinitionLoader.LoadFromJson(
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
                LanguageDefinitionLoader.LoadFromJson(
                    """
                    [
                      {
                        "id": "extension.language.test",
                        "name": "Test",
                        "category": "Standard",
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
                LanguageDefinitionLoader.LoadFromJson(
                    """
                    [
                      {
                        "id": "extension.language.test",
                        "id": "extension.language.other",
                        "name": "Test",
                        "category": "Standard",
                        "sources": []
                      }
                    ]
                    """));
    }

    [Fact]
    public void MissingRequiredCategoryMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () =>
                LanguageDefinitionLoader.LoadFromJson(
                    """
                    [
                      {
                        "id": "extension.language.test",
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
                LanguageDefinitionLoader.LoadFromJson(
                    """
                    [
                      {
                        "id": "extension.language.test",
                        "name": null,
                        "category": "Standard",
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
                LanguageDefinitionLoader.LoadFromJson(
                    """
                    [
                      {
                        "id": "extension.language.test",
                        "name": "Test",
                        "category": "Standard",
                        "sources": null
                      }
                    ]
                    """));
    }

    [Fact]
    public void IntegerCategory_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () =>
                LanguageDefinitionLoader.LoadFromJson(
                    """
                    [
                      {
                        "id": "extension.language.test",
                        "name": "Test",
                        "category": 0,
                        "sources": []
                      }
                    ]
                    """));
    }

    [Fact]
    public void UnknownStringCategory_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () =>
                LanguageDefinitionLoader.LoadFromJson(
                    """
                    [
                      {
                        "id": "extension.language.test",
                        "name": "Test",
                        "category": "Regional",
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
              "id": "extension.language.test",
              "name": "Test",
              "category": "Exotic",
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
                LanguageDefinitionLoader.LoadFromJson(
                    json));
    }
}
