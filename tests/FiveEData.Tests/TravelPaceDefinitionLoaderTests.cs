using FiveEData.Rules.Adventuring.TravelPace;
using FiveEData.Rules.Adventuring.TravelPace.Serialization;

namespace FiveEData.Tests;

public sealed class TravelPaceDefinitionLoaderTests
{
    [Fact]
    public void ValidDefinition_LoadsStrictly()
    {
        TravelPaceDefinition definition = Assert.Single(
            TravelPaceDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.travel-pace.test",
                    "name": "Test",
                    "feetPerMinute": 300,
                    "milesPerHour": 3,
                    "milesPerDay": 24,
                    "passiveWisdomPerceptionPenalty": null,
                    "allowsStealth": false,
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

        Assert.Equal("extension.travel-pace.test", definition.Id.Value);
        Assert.Equal("Test", definition.Name);
        Assert.Equal(300, definition.FeetPerMinute);
        Assert.Equal(3, definition.MilesPerHour);
        Assert.Equal(24, definition.MilesPerDay);
        Assert.Null(definition.PassiveWisdomPerceptionPenalty);
        Assert.False(definition.AllowsStealth);
        Assert.Single(definition.Sources);
    }

    [Fact]
    public void NullRoot_IsRejected()
    {
        Assert.ThrowsAny<Exception>(
            () => TravelPaceDefinitionLoader.LoadFromJson("null"));
    }

    [Fact]
    public void NullElement_IsRejected()
    {
        Assert.ThrowsAny<Exception>(
            () => TravelPaceDefinitionLoader.LoadFromJson("[null]"));
    }

    [Fact]
    public void UnknownProperty_IsRejected()
    {
        Assert.ThrowsAny<Exception>(
            () => TravelPaceDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.travel-pace.test",
                    "name": "Test",
                    "feetPerMinute": 300,
                    "milesPerHour": 3,
                    "milesPerDay": 24,
                    "passiveWisdomPerceptionPenalty": null,
                    "allowsStealth": false,
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
            () => TravelPaceDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.travel-pace.test",
                    "id": "extension.travel-pace.other",
                    "name": "Test",
                    "feetPerMinute": 300,
                    "milesPerHour": 3,
                    "milesPerDay": 24,
                    "passiveWisdomPerceptionPenalty": null,
                    "allowsStealth": false,
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
                "id": "extension.travel-pace.test",
                "name": "Test",
                "feetPerMinute": 300,
                "milesPerHour": 3,
                "milesPerDay": 24,
                "passiveWisdomPerceptionPenalty": null,
                "allowsStealth": false,
                "sources": [
                  { "documentId": "extension.source.test" }
                ],
                {{nulledMember}}
              }
            ]
            """;

        Assert.ThrowsAny<Exception>(
            () => TravelPaceDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void MissingRequiredMember_IsRejected()
    {
        Assert.ThrowsAny<Exception>(
            () => TravelPaceDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.travel-pace.test",
                    "name": "Test"
                  }
                ]
                """));
    }

    [Fact]
    public void DuplicateIds_AreRejected()
    {
        Assert.ThrowsAny<Exception>(
            () => TravelPaceDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.travel-pace.test",
                    "name": "Test",
                    "feetPerMinute": 300,
                    "milesPerHour": 3,
                    "milesPerDay": 24,
                    "passiveWisdomPerceptionPenalty": null,
                    "allowsStealth": false,
                    "sources": [
                      { "documentId": "extension.source.test" }
                    ]
                  },
                  {
                    "id": "extension.travel-pace.test",
                    "name": "Test",
                    "feetPerMinute": 300,
                    "milesPerHour": 3,
                    "milesPerDay": 24,
                    "passiveWisdomPerceptionPenalty": null,
                    "allowsStealth": false,
                    "sources": [
                      { "documentId": "extension.source.test" }
                    ]
                  }
                ]
                """));
    }
}
