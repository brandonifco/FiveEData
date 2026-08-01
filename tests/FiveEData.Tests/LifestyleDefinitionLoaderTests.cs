using FiveEData.Rules.Common;
using FiveEData.Rules.Expenses.Lifestyles;
using FiveEData.Rules.Expenses.Lifestyles.Serialization;

namespace FiveEData.Tests;

public sealed class LifestyleDefinitionLoaderTests
{
    [Fact]
    public void ValidExactCost_IsLoaded()
    {
        LifestyleDefinition definition =
            Assert.Single(
                LifestyleDefinitionLoader.LoadFromJson(
                    CreateJson(
                        """
                        {"amount":{"copperPieces":100},"kind":"Exact"}
                        """)));

        Assert.Equal(
            "dnd5e2014.lifestyle.test",
            definition.Id.Value);
        Assert.Equal("Test lifestyle", definition.Name);
        Assert.Equal(
            100,
            definition.DailyCost?.Amount.CopperPieces);
        Assert.Equal(
            ListedCostKind.Exact,
            definition.DailyCost?.Kind);
    }

    [Fact]
    public void ExplicitNullDailyCost_IsLoaded()
    {
        LifestyleDefinition definition =
            Assert.Single(
                LifestyleDefinitionLoader.LoadFromJson(
                    CreateJson("null")));

        Assert.Null(definition.DailyCost);
    }

    [Fact]
    public void MinimumCost_IsLoaded()
    {
        LifestyleDefinition definition =
            Assert.Single(
                LifestyleDefinitionLoader.LoadFromJson(
                    CreateJson(
                        """
                        {"amount":{"copperPieces":1000},"kind":"Minimum"}
                        """)));

        Assert.Equal(
            ListedCostKind.Minimum,
            definition.DailyCost?.Kind);
    }

    [Fact]
    public void MissingDailyCostMember_IsRejected()
    {
        const string json =
            """
            [
              {
                "id": "dnd5e2014.lifestyle.test",
                "name": "Test lifestyle",
                "specialRuleIds": [],
                "sources": [
                  {
                    "documentId": "dnd5e2014.source.phb-first-printing",
                    "page": 157,
                    "section": "Lifestyle Expenses"
                  }
                ]
              }
            ]
            """;

        Assert.Throws<InvalidDataException>(
            () => LifestyleDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void UnknownMember_IsRejected()
    {
        string json = CreateJson(
            """
            {"amount":{"copperPieces":100},"kind":"Exact"}
            """).Replace(
                """
                "specialRuleIds": [],
                """,
                """
                "unexpected": true,
                "specialRuleIds": [],
                """,
                StringComparison.Ordinal);

        Assert.Throws<InvalidDataException>(
            () => LifestyleDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void DuplicateIds_AreRejected()
    {
        string item = CreateJson(
            """
            {"amount":{"copperPieces":100},"kind":"Exact"}
            """).Trim();

        string objectJson = item[1..^1].Trim();

        string json =
            $"[{objectJson},{objectJson}]";

        Assert.Throws<InvalidDataException>(
            () => LifestyleDefinitionLoader.LoadFromJson(json));
    }

    private static string CreateJson(string dailyCost)
    {
        return
            $$"""
            [
              {
                "id": "dnd5e2014.lifestyle.test",
                "name": "Test lifestyle",
                "dailyCost": {{dailyCost}},
                "specialRuleIds": [],
                "sources": [
                  {
                    "documentId": "dnd5e2014.source.phb-first-printing",
                    "page": 157,
                    "section": "Lifestyle Expenses"
                  }
                ]
              }
            ]
            """;
    }
}
