using FiveEData.Rules.Common;
using FiveEData.Rules.Expenses.Services;
using FiveEData.Rules.Expenses.Services.Serialization;

namespace FiveEData.Tests;

public sealed class MundaneServiceDefinitionLoaderTests
{
    [Fact]
    public void ValidExactCost_IsLoaded()
    {
        MundaneServiceDefinition definition =
            Assert.Single(
                MundaneServiceDefinitionLoader.LoadFromJson(
                    CreateJson(
                        """
                        {
                          "amount": {
                            "copperPieces": 3
                          },
                          "kind": "Exact"
                        }
                        """,
                        "\"Mile\"")));

        Assert.Equal(
            "dnd5e2014.mundane-service.test",
            definition.Id.Value);
        Assert.Equal("Test service", definition.Name);
        Assert.Equal(
            3,
            definition.Cost.Amount.CopperPieces);
        Assert.Equal(
            ListedCostKind.Exact,
            definition.Cost.Kind);
        Assert.Equal(
            ServicePricingUnit.Mile,
            definition.PricingUnit);
    }

    [Fact]
    public void MinimumCost_IsLoaded()
    {
        MundaneServiceDefinition definition =
            Assert.Single(
                MundaneServiceDefinitionLoader.LoadFromJson(
                    CreateJson(
                        """
                        {
                          "amount": {
                            "copperPieces": 200
                          },
                          "kind": "Minimum"
                        }
                        """,
                        "\"Day\"")));

        Assert.Equal(
            ListedCostKind.Minimum,
            definition.Cost.Kind);
    }

    [Fact]
    public void MissingCost_IsRejected()
    {
        string json = CreateJson(
            """
            {
              "amount": {
                "copperPieces": 3
              },
              "kind": "Exact"
            }
            """,
            "\"Mile\"").Replace(
                """
                "cost": {
                  "amount": {
                    "copperPieces": 3
                  },
                  "kind": "Exact"
                },
                """,
                string.Empty,
                StringComparison.Ordinal);

        Assert.Throws<InvalidDataException>(
            () => MundaneServiceDefinitionLoader
                .LoadFromJson(json));
    }

    [Fact]
    public void UnknownMember_IsRejected()
    {
        string json = CreateJson(
            """
            {
              "amount": {
                "copperPieces": 3
              },
              "kind": "Exact"
            }
            """,
            "\"Mile\"").Replace(
                """
                "specialRuleIds": [],
                """,
                """
                "unexpected": true,
                "specialRuleIds": [],
                """,
                StringComparison.Ordinal);

        Assert.Throws<InvalidDataException>(
            () => MundaneServiceDefinitionLoader
                .LoadFromJson(json));
    }

    [Fact]
    public void IntegerPricingUnit_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => MundaneServiceDefinitionLoader.LoadFromJson(
                CreateJson(
                    """
                    {
                      "amount": {
                        "copperPieces": 3
                      },
                      "kind": "Exact"
                    }
                    """,
                    "2")));
    }

    [Fact]
    public void UnknownPricingUnit_IsRejected()
    {
        Assert.Throws<InvalidDataException>(
            () => MundaneServiceDefinitionLoader.LoadFromJson(
                CreateJson(
                    """
                    {
                      "amount": {
                        "copperPieces": 3
                      },
                      "kind": "Exact"
                    }
                    """,
                    "\"Unknown\"")));
    }

    [Fact]
    public void DuplicateIds_AreRejected()
    {
        string item = CreateJson(
            """
            {
              "amount": {
                "copperPieces": 3
              },
              "kind": "Exact"
            }
            """,
            "\"Mile\"").Trim();

        string objectJson = item[1..^1].Trim();

        Assert.Throws<InvalidDataException>(
            () => MundaneServiceDefinitionLoader
                .LoadFromJson(
                    $"[{objectJson},{objectJson}]"));
    }

    [Fact]
    public void NullSpecialRuleIds_AreRejected()
    {
        string json = CreateJson(
            """
            {
              "amount": {
                "copperPieces": 3
              },
              "kind": "Exact"
            }
            """,
            "\"Mile\"").Replace(
                "\"specialRuleIds\": []",
                "\"specialRuleIds\": null",
                StringComparison.Ordinal);

        Assert.Throws<InvalidDataException>(
            () => MundaneServiceDefinitionLoader
                .LoadFromJson(json));
    }

    [Fact]
    public void NullSources_AreRejected()
    {
        const string json =
            """
            [
              {
                "id": "dnd5e2014.mundane-service.test",
                "name": "Test service",
                "cost": {
                  "amount": {
                    "copperPieces": 3
                  },
                  "kind": "Exact"
                },
                "pricingUnit": "Mile",
                "specialRuleIds": [],
                "sources": null
              }
            ]
            """;

        Assert.Throws<InvalidDataException>(
            () => MundaneServiceDefinitionLoader
                .LoadFromJson(json));
    }

    private static string CreateJson(
        string cost,
        string pricingUnit)
    {
        return
            $$"""
            [
              {
                "id": "dnd5e2014.mundane-service.test",
                "name": "Test service",
                "cost": {{cost}},
                "pricingUnit": {{pricingUnit}},
                "specialRuleIds": [],
                "sources": [
                  {
                    "documentId": "dnd5e2014.source.phb-first-printing",
                    "page": 159,
                    "section": "Services"
                  }
                ]
              }
            ]
            """;
    }
}
