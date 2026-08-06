using FiveEData.Rules.Classes.EldritchInvocations;
using FiveEData.Rules.Classes.EldritchInvocations.Serialization;

namespace FiveEData.Tests;

public sealed class EldritchInvocationDefinitionLoaderTests
{
    [Fact]
    public void ValidDefinition_LoadsStrictlyWithNoPrerequisites()
    {
        EldritchInvocationDefinition definition = Assert.Single(
            EldritchInvocationDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.eldritch-invocation.test",
                    "name": "Test",
                    "requiresEldritchBlastCantrip": false,
                    "requiredMinimumLevel": null,
                    "requiresPactBoon": null,
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
            "extension.eldritch-invocation.test",
            definition.Id.Value);
        Assert.Equal("Test", definition.Name);
        Assert.False(definition.RequiresEldritchBlastCantrip);
        Assert.Null(definition.RequiredMinimumLevel);
        Assert.Null(definition.RequiresPactBoon);
        Assert.Single(definition.Sources);
    }

    [Fact]
    public void ValidDefinition_LoadsAllPrerequisitesWhenPresent()
    {
        EldritchInvocationDefinition definition = Assert.Single(
            EldritchInvocationDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.eldritch-invocation.test",
                    "name": "Test",
                    "requiresEldritchBlastCantrip": true,
                    "requiredMinimumLevel": 12,
                    "requiresPactBoon": "Blade",
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

        Assert.True(definition.RequiresEldritchBlastCantrip);
        Assert.Equal(12, definition.RequiredMinimumLevel);
        Assert.Equal(WarlockPactBoon.Blade, definition.RequiresPactBoon);
    }

    [Fact]
    public void InvalidRequiresPactBoonValue_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => EldritchInvocationDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.eldritch-invocation.test",
                    "name": "Test",
                    "requiresEldritchBlastCantrip": false,
                    "requiredMinimumLevel": null,
                    "requiresPactBoon": "NotARealPactBoon",
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void NullRootArray_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => EldritchInvocationDefinitionLoader.LoadFromJson("null"));
    }

    [Fact]
    public void NullArrayElement_IsRejectedAsDataError()
    {
        InvalidDataException exception =
            Assert.Throws<InvalidDataException>(
                () => EldritchInvocationDefinitionLoader.LoadFromJson(
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
            () => EldritchInvocationDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.eldritch-invocation.test",
                    "name": "Test",
                    "requiresEldritchBlastCantrip": false,
                    "requiredMinimumLevel": null,
                    "requiresPactBoon": null,
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
            () => EldritchInvocationDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.eldritch-invocation.test",
                    "name": "Test",
                    "name": "Other",
                    "requiresEldritchBlastCantrip": false,
                    "requiredMinimumLevel": null,
                    "requiresPactBoon": null,
                    "sources": []
                  }
                ]
                """));
    }

    [Fact]
    public void MissingRequiredSourcesMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => EldritchInvocationDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.eldritch-invocation.test",
                    "name": "Test",
                    "requiresEldritchBlastCantrip": false,
                    "requiredMinimumLevel": null,
                    "requiresPactBoon": null
                  }
                ]
                """));
    }

    [Fact]
    public void NullRequiredIdMember_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => EldritchInvocationDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": null,
                    "name": "Test",
                    "requiresEldritchBlastCantrip": false,
                    "requiredMinimumLevel": null,
                    "requiresPactBoon": null,
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
              "id": "extension.eldritch-invocation.test",
              "name": "Test",
              "requiresEldritchBlastCantrip": false,
              "requiredMinimumLevel": null,
              "requiresPactBoon": null,
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
            () => EldritchInvocationDefinitionLoader.LoadFromJson(json));
    }
}
