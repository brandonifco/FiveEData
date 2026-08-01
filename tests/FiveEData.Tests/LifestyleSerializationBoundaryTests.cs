using FiveEData.Rules.Expenses.Lifestyles.Serialization;

namespace FiveEData.Tests;

public sealed class LifestyleSerializationBoundaryTests
{
    [Fact]
    public void IntegerListedCostKind_IsRejected()
    {
        string json = CreateJson(
            """
            {
              "amount": {
                "copperPieces": 100
              },
              "kind": 1
            }
            """);

        Assert.Throws<InvalidDataException>(
            () => LifestyleDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void UnknownListedCostKind_IsRejected()
    {
        string json = CreateJson(
            """
            {
              "amount": {
                "copperPieces": 100
              },
              "kind": "Suggested"
            }
            """);

        Assert.Throws<InvalidDataException>(
            () => LifestyleDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void MissingListedCostAmount_IsRejected()
    {
        string json = CreateJson(
            """
            {
              "kind": "Exact"
            }
            """);

        Assert.Throws<InvalidDataException>(
            () => LifestyleDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void UnknownNestedListedCostMember_IsRejected()
    {
        string json = CreateJson(
            """
            {
              "amount": {
                "copperPieces": 100
              },
              "kind": "Exact",
              "unexpected": true
            }
            """);

        Assert.Throws<InvalidDataException>(
            () => LifestyleDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void NullSpecialRuleIds_IsRejected()
    {
        string json = CreateJson(
            """
            {
              "amount": {
                "copperPieces": 100
              },
              "kind": "Exact"
            }
            """).Replace(
                """
                "specialRuleIds": [],
                """,
                """
                "specialRuleIds": null,
                """,
                StringComparison.Ordinal);

        Assert.Throws<InvalidDataException>(
            () => LifestyleDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void DuplicateSpecialRuleIds_AreRejected()
    {
        string json = CreateJson(
            """
            {
              "amount": {
                "copperPieces": 100
              },
              "kind": "Exact"
            }
            """).Replace(
                """
                "specialRuleIds": [],
                """,
                """
                "specialRuleIds": [
                  "dnd5e2014.lifestyle-rule.test",
                  "dnd5e2014.lifestyle-rule.test"
                ],
                """,
                StringComparison.Ordinal);

        Assert.Throws<InvalidDataException>(
            () => LifestyleDefinitionLoader.LoadFromJson(json));
    }

    [Fact]
    public void NullSources_IsRejected()
    {
        const string json =
            """
            [
              {
                "id": "dnd5e2014.lifestyle.test",
                "name": "Test lifestyle",
                "dailyCost": {
                  "amount": {
                    "copperPieces": 100
                  },
                  "kind": "Exact"
                },
                "specialRuleIds": [],
                "sources": null
              }
            ]
            """;

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
