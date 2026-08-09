using FiveEData.Rules.Adventuring.DowntimeActivities;
using FiveEData.Rules.Adventuring.DowntimeActivities.Serialization;

namespace FiveEData.Tests;

public sealed class DowntimeActivityDefinitionLoaderTests
{
    [Fact]
    public void ValidDefinition_LoadsStrictly()
    {
        DowntimeActivityDefinition definition = Assert.Single(
            DowntimeActivityDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.downtime-activity.test",
                    "name": "Test",
                    "requiredDays": 3,
                    "costPerDayGoldPieces": null,
                    "savingThrowAbilityId": "dnd5e2014.ability.constitution",
                    "savingThrowDC": 15,
                    "marketValueProgressPerDayGoldPieces": null,
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

        Assert.Equal("extension.downtime-activity.test", definition.Id.Value);
        Assert.Equal("Test", definition.Name);
        Assert.Equal(3, definition.RequiredDays);
        Assert.Null(definition.CostPerDayGoldPieces);
        Assert.Equal(
            "dnd5e2014.ability.constitution",
            definition.SavingThrowAbilityId!.Value.Value);
        Assert.Equal(15, definition.SavingThrowDC);
        Assert.Null(definition.MarketValueProgressPerDayGoldPieces);
        Assert.Single(definition.Sources);
    }

    [Fact]
    public void ValidDefinition_LoadsAllFactsDeclined()
    {
        DowntimeActivityDefinition definition = Assert.Single(
            DowntimeActivityDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.downtime-activity.test",
                    "name": "Test",
                    "requiredDays": null,
                    "costPerDayGoldPieces": null,
                    "savingThrowAbilityId": null,
                    "savingThrowDC": null,
                    "marketValueProgressPerDayGoldPieces": null,
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

        Assert.Null(definition.RequiredDays);
        Assert.Null(definition.CostPerDayGoldPieces);
        Assert.Null(definition.SavingThrowAbilityId);
        Assert.Null(definition.SavingThrowDC);
        Assert.Null(definition.MarketValueProgressPerDayGoldPieces);
    }

    [Fact]
    public void NullRoot_IsRejected()
    {
        Assert.ThrowsAny<Exception>(
            () => DowntimeActivityDefinitionLoader.LoadFromJson("null"));
    }

    [Fact]
    public void NullElement_IsRejected()
    {
        Assert.ThrowsAny<Exception>(
            () => DowntimeActivityDefinitionLoader.LoadFromJson("[null]"));
    }

    [Fact]
    public void UnknownProperty_IsRejected()
    {
        Assert.ThrowsAny<Exception>(
            () => DowntimeActivityDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.downtime-activity.test",
                    "name": "Test",
                    "requiredDays": null,
                    "costPerDayGoldPieces": null,
                    "savingThrowAbilityId": null,
                    "savingThrowDC": null,
                    "marketValueProgressPerDayGoldPieces": null,
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
            () => DowntimeActivityDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.downtime-activity.test",
                    "id": "extension.downtime-activity.other",
                    "name": "Test",
                    "requiredDays": null,
                    "costPerDayGoldPieces": null,
                    "savingThrowAbilityId": null,
                    "savingThrowDC": null,
                    "marketValueProgressPerDayGoldPieces": null,
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
                "id": "extension.downtime-activity.test",
                "name": "Test",
                "requiredDays": null,
                "costPerDayGoldPieces": null,
                "savingThrowAbilityId": null,
                "savingThrowDC": null,
                "marketValueProgressPerDayGoldPieces": null,
                "sources": [
                  { "documentId": "extension.source.test" }
                ],
                {{nulledMember}}
              }
            ]
            """;

        Assert.ThrowsAny<Exception>(
            () => DowntimeActivityDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void MissingRequiredMember_IsRejected()
    {
        Assert.ThrowsAny<Exception>(
            () => DowntimeActivityDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.downtime-activity.test",
                    "name": "Test"
                  }
                ]
                """));
    }

    [Fact]
    public void DuplicateIds_AreRejected()
    {
        Assert.ThrowsAny<Exception>(
            () => DowntimeActivityDefinitionLoader.LoadFromJson(
                """
                [
                  {
                    "id": "extension.downtime-activity.test",
                    "name": "Test",
                    "requiredDays": null,
                    "costPerDayGoldPieces": null,
                    "savingThrowAbilityId": null,
                    "savingThrowDC": null,
                    "marketValueProgressPerDayGoldPieces": null,
                    "sources": [
                      { "documentId": "extension.source.test" }
                    ]
                  },
                  {
                    "id": "extension.downtime-activity.test",
                    "name": "Test",
                    "requiredDays": null,
                    "costPerDayGoldPieces": null,
                    "savingThrowAbilityId": null,
                    "savingThrowDC": null,
                    "marketValueProgressPerDayGoldPieces": null,
                    "sources": [
                      { "documentId": "extension.source.test" }
                    ]
                  }
                ]
                """));
    }
}
