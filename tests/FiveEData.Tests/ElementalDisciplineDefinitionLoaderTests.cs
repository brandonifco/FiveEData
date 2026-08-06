using FiveEData.Rules.Classes.ElementalDisciplines;
using FiveEData.Rules.Classes.ElementalDisciplines.Serialization;

namespace FiveEData.Tests;

public sealed class ElementalDisciplineDefinitionLoaderTests
{
    [Fact]
    public void ValidDefinition_LoadsStrictlyWithKiCostAndLevel()
    {
        ElementalDisciplineDefinition definition = Assert.Single(
            ElementalDisciplineDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.elemental-discipline.test",
                    "name": "Test",
                    "kiPointCost": 3,
                    "requiredMinimumLevel": 6,
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
            "extension.elemental-discipline.test",
            definition.Id.Value);
        Assert.Equal("Test", definition.Name);
        Assert.Equal(3, definition.KiPointCost);
        Assert.Equal(6, definition.RequiredMinimumLevel);
        Assert.Single(definition.Sources);
    }

    [Fact]
    public void ValidDefinition_LoadsWithNoKiCostAndNoLevel()
    {
        ElementalDisciplineDefinition definition = Assert.Single(
            ElementalDisciplineDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.elemental-discipline.test",
                    "name": "Test",
                    "kiPointCost": null,
                    "requiredMinimumLevel": null,
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

        Assert.Null(definition.KiPointCost);
        Assert.Null(definition.RequiredMinimumLevel);
    }

    [Fact]
    public void NullRootArray_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => ElementalDisciplineDefinitionLoader.LoadFromJson("null"));
    }

    [Fact]
    public void NullArrayElement_IsRejectedAsDataError()
    {
        InvalidDataException exception =
            Assert.Throws<InvalidDataException>(
                () => ElementalDisciplineDefinitionLoader.LoadFromJson(
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
            () => ElementalDisciplineDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.elemental-discipline.test",
                    "name": "Test",
                    "kiPointCost": 2,
                    "requiredMinimumLevel": null,
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
            () => ElementalDisciplineDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.elemental-discipline.test",
                    "name": "Test",
                    "name": "Other",
                    "kiPointCost": 2,
                    "requiredMinimumLevel": null,
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void MissingRequiredSourcesMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => ElementalDisciplineDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.elemental-discipline.test",
                    "name": "Test",
                    "kiPointCost": 2,
                    "requiredMinimumLevel": null
                  }
                ]
                """));
    }

    [Fact]
    public void NullRequiredIdMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => ElementalDisciplineDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": null,
                    "name": "Test",
                    "kiPointCost": 2,
                    "requiredMinimumLevel": null,
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
              "id": "extension.elemental-discipline.test",
              "name": "Test",
              "kiPointCost": 2,
              "requiredMinimumLevel": null,
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
            () => ElementalDisciplineDefinitionLoader.LoadFromJson(json));
    }
}
