using FiveEData.Rules.Classes.ThirdEyeOptions;
using FiveEData.Rules.Classes.ThirdEyeOptions.Serialization;

namespace FiveEData.Tests;

public sealed class ThirdEyeOptionDefinitionLoaderTests
{
    private const string MinimalMembers =
        """
        "darkvisionRangeFeet": null,
        "etherealSightRangeFeet": null,
        "seeInvisibilityRangeFeet": null,
        "canReadAllLanguages": false
        """;

    private const string TestSources =
        """
        "sources": [
          {
            "documentId": "extension.source.test",
            "page": 1,
            "section": "Test section"
          }
        ]
        """;

    [Fact]
    public void ValidDefinition_LoadsStrictly()
    {
        ThirdEyeOptionDefinition definition = Assert.Single(
            ThirdEyeOptionDefinitionLoader.LoadFromJson(
                $$"""
                [
                  {
                    "id": "extension.third-eye-option.test",
                    "name": "Test",
                    {{MinimalMembers}},
                    {{TestSources}}
                  }
                ]
                """));

        Assert.Equal("extension.third-eye-option.test", definition.Id.Value);
        Assert.Equal("Test", definition.Name);
        Assert.Single(definition.Sources);
    }

    [Fact]
    public void ValidDefinition_LoadsMechanismFieldsWhenPresent()
    {
        ThirdEyeOptionDefinition definition = Assert.Single(
            ThirdEyeOptionDefinitionLoader.LoadFromJson(
                $$"""
                [
                  {
                    "id": "extension.third-eye-option.test",
                    "name": "Test",
                    "darkvisionRangeFeet": 60,
                    "etherealSightRangeFeet": 60,
                    "seeInvisibilityRangeFeet": 10,
                    "canReadAllLanguages": true,
                    {{TestSources}}
                  }
                ]
                """));

        Assert.Equal(60, definition.DarkvisionRangeFeet);
        Assert.Equal(60, definition.EtherealSightRangeFeet);
        Assert.Equal(10, definition.SeeInvisibilityRangeFeet);
        Assert.True(definition.CanReadAllLanguages);
    }

    [Fact]
    public void NullRootArray_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => ThirdEyeOptionDefinitionLoader.LoadFromJson("null"));
    }

    [Fact]
    public void NullArrayElement_IsRejectedAsDataError()
    {
        InvalidDataException exception =
            Assert.Throws<InvalidDataException>(
                () => ThirdEyeOptionDefinitionLoader.LoadFromJson("[null]"));

        Assert.Contains(
            "index 0",
            exception.Message,
            StringComparison.Ordinal);
    }

    [Fact]
    public void UnknownProperty_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => ThirdEyeOptionDefinitionLoader.LoadFromJson(
                $$"""
                [
                  {
                    "id": "extension.third-eye-option.test",
                    "name": "Test",
                    {{MinimalMembers}},
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
            () => ThirdEyeOptionDefinitionLoader.LoadFromJson(
                $$"""
                [
                  {
                    "id": "extension.third-eye-option.test",
                    "name": "Test",
                    "name": "Other",
                    {{MinimalMembers}},
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void MissingRequiredSourcesMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => ThirdEyeOptionDefinitionLoader.LoadFromJson(
                $$"""
                [
                  {
                    "id": "extension.third-eye-option.test",
                    "name": "Test",
                    {{MinimalMembers}}
                  }
                ]
                """));
    }

    [Fact]
    public void NullRequiredIdMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => ThirdEyeOptionDefinitionLoader.LoadFromJson(
                $$"""
                [
                  {
                    "id": null,
                    "name": "Test",
                    {{MinimalMembers}},
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void DuplicateIds_AreRejected()
    {
        string one =
            $$"""
            {
              "id": "extension.third-eye-option.test",
              "name": "Test",
              {{MinimalMembers}},
              {{TestSources}}
            }
            """;

        Assert.Throws<InvalidDataException>(
            () => ThirdEyeOptionDefinitionLoader.LoadFromJson($"[{one},{one}]"));
    }
}
