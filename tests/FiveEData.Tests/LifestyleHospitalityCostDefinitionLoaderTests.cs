using FiveEData.Rules.Expenses.FoodAndLodging;
using FiveEData.Rules.Expenses.FoodAndLodging.Serialization;

namespace FiveEData.Tests;

public sealed class
    LifestyleHospitalityCostDefinitionLoaderTests
{
    [Fact]
    public void ValidDefinition_IsLoaded()
    {
        LifestyleHospitalityCostDefinition definition =
            Assert.Single(
                LifestyleHospitalityCostDefinitionLoader
                    .LoadFromJson(CreateJson()));

        Assert.Equal(
            "dnd5e2014.lifestyle.modest",
            definition.LifestyleId.Value);
        Assert.Equal(
            50,
            definition.InnStayCostPerDay.CopperPieces);
        Assert.Equal(
            30,
            definition.MealsCostPerDay.CopperPieces);
    }

    [Fact]
    public void MissingInnStayCost_IsRejected()
    {
        string json = CreateJson().Replace(
            """
                "innStayCostPerDay": {
                  "copperPieces": 50
                },
            """,
            string.Empty,
            StringComparison.Ordinal);

        Assert.Throws<InvalidDataException>(
            () =>
                LifestyleHospitalityCostDefinitionLoader
                    .LoadFromJson(json));
    }

    [Fact]
    public void MissingMealsCost_IsRejected()
    {
        string json = CreateJson().Replace(
            """
                "mealsCostPerDay": {
                  "copperPieces": 30
                },
            """,
            string.Empty,
            StringComparison.Ordinal);

        Assert.Throws<InvalidDataException>(
            () =>
                LifestyleHospitalityCostDefinitionLoader
                    .LoadFromJson(json));
    }

    [Fact]
    public void UnknownMember_IsRejected()
    {
        string json = CreateJson().Replace(
            """
                "specialRuleIds": [],
            """,
            """
                "unexpected": true,
                "specialRuleIds": [],
            """,
            StringComparison.Ordinal);

        Assert.Throws<InvalidDataException>(
            () =>
                LifestyleHospitalityCostDefinitionLoader
                    .LoadFromJson(json));
    }

    [Fact]
    public void DuplicateLifestyleIds_AreRejected()
    {
        string item = CreateJson().Trim();
        string objectJson = item[1..^1].Trim();

        string json = $"[{objectJson},{objectJson}]";

        Assert.Throws<InvalidDataException>(
            () =>
                LifestyleHospitalityCostDefinitionLoader
                    .LoadFromJson(json));
    }

    [Fact]
    public void NullSpecialRuleIds_AreRejected()
    {
        string json = CreateJson().Replace(
            """
                "specialRuleIds": [],
            """,
            """
                "specialRuleIds": null,
            """,
            StringComparison.Ordinal);

        Assert.Throws<InvalidDataException>(
            () =>
                LifestyleHospitalityCostDefinitionLoader
                    .LoadFromJson(json));
    }

    [Fact]
    public void NullSources_AreRejected()
    {
        const string json =
            """
            [
              {
                "lifestyleId": "dnd5e2014.lifestyle.modest",
                "innStayCostPerDay": {
                  "copperPieces": 50
                },
                "mealsCostPerDay": {
                  "copperPieces": 30
                },
                "specialRuleIds": [],
                "sources": null
              }
            ]
            """;

        Assert.Throws<InvalidDataException>(
            () =>
                LifestyleHospitalityCostDefinitionLoader
                    .LoadFromJson(json));
    }

    private static string CreateJson()
    {
        return
            """
            [
              {
                "lifestyleId": "dnd5e2014.lifestyle.modest",
                "innStayCostPerDay": {
                  "copperPieces": 50
                },
                "mealsCostPerDay": {
                  "copperPieces": 30
                },
                "specialRuleIds": [],
                "sources": [
                  {
                    "documentId": "dnd5e2014.source.phb-first-printing",
                    "page": 158,
                    "section": "Food, Drink, and Lodging"
                  }
                ]
              }
            ]
            """;
    }
}
