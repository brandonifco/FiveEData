using FiveEData.Rules.Equipment.AdventuringGear;
using FiveEData.Rules.Equipment.AdventuringGear.Serialization;

namespace FiveEData.Tests;

public sealed class AdventuringGearDefinitionLoaderTests
{
    [Fact]
    public void ValidJson_MapsToDomainDefinition()
    {
        AdventuringGearDefinition definition = Assert.Single(
            AdventuringGearDefinitionLoader.LoadFromJson(ValidJson));

        Assert.Equal(
            new AdventuringGearId("dnd5e2014.adventuring-gear.test"),
            definition.Id);
        Assert.Equal("Test gear", definition.Name);
        Assert.Equal(200, definition.Cost.CopperPieces);
        Assert.NotNull(definition.ListedWeight);
        Assert.Equal(5m, definition.ListedWeight.Weight.Pounds);
        Assert.Equal("full", definition.ListedWeight.Qualifier);
        Assert.Empty(definition.SpecialRuleIds);
        Assert.Single(definition.Sources);
    }

    [Fact]
    public void ExplicitNullListedWeight_IsAccepted()
    {
        string json = ValidJson.Replace(
            "\"listedWeight\": {\n      \"pounds\": 5,\n      \"qualifier\": \"full\"\n    }",
            "\"listedWeight\": null",
            StringComparison.Ordinal);

        AdventuringGearDefinition definition = Assert.Single(
            AdventuringGearDefinitionLoader.LoadFromJson(json));

        Assert.Null(definition.ListedWeight);
    }

    [Fact]
    public void UnknownJsonMember_IsRejected()
    {
        string json = ValidJson.Replace(
            "\"specialRuleIds\": [],",
            "\"specialRuleIds\": [],\n    \"unexpected\": true,",
            StringComparison.Ordinal);

        Assert.Throws<InvalidDataException>(
            () => AdventuringGearDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void MissingRequiredListedWeightMember_IsRejected()
    {
        string json = ValidJson.Replace(
            "    \"listedWeight\": {\n      \"pounds\": 5,\n      \"qualifier\": \"full\"\n    },\n",
            string.Empty,
            StringComparison.Ordinal);

        Assert.Throws<InvalidDataException>(
            () => AdventuringGearDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void InvalidListedWeight_IsRejected()
    {
        string json = ValidJson.Replace(
            "\"pounds\": 5",
            "\"pounds\": 0",
            StringComparison.Ordinal);

        Assert.Throws<InvalidDataException>(
            () => AdventuringGearDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void NullArrayElement_IsRejectedAsDataError()
    {
        InvalidDataException exception = Assert.Throws<InvalidDataException>(
            () => AdventuringGearDefinitionLoader.LoadFromJson("[null]"));

        Assert.Contains("index 0", exception.Message);
    }

    [Fact]
    public void DuplicateIds_AreRejected()
    {
        string item = ValidJson[2..^2];
        string json = $"[{item},{item}]";

        Assert.Throws<InvalidDataException>(
            () => AdventuringGearDefinitionLoader.LoadFromJson(json));
    }

    private const string ValidJson = """
    [
      {
        "id": "dnd5e2014.adventuring-gear.test",
        "name": "Test gear",
        "cost": {
          "copperPieces": 200
        },
        "listedWeight": {
          "pounds": 5,
          "qualifier": "full"
        },
        "specialRuleIds": [],
        "sources": [
          {
            "documentId": "dnd5e2014.source.phb-first-printing",
            "page": 150,
            "section": "Chapter 5: Equipment — Adventuring Gear"
          }
        ]
      }
    ]
    """;
}
