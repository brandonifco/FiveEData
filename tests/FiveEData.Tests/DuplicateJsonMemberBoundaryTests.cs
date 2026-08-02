using System.Text.Json;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Expenses.FoodAndLodging.Serialization;
using FiveEData.Rules.Expenses.Lifestyles.Serialization;
using FiveEData.Rules.Expenses.Services.Serialization;

namespace FiveEData.Tests;

public sealed class DuplicateJsonMemberBoundaryTests
{
    [Fact]
    public void SharedObjectBoundary_RejectsDuplicateRootMember()
    {
        const string json =
            "{\"value\":1,\"value\":2}";

        AssertDuplicateMemberRejected(
            () =>
                StrictJson.DeserializeObject<
                    Dictionary<string, object?>>(
                        json,
                        "Test object"),
            "value");
    }

    [Fact]
    public void SharedObjectBoundary_RejectsDuplicateNestedMember()
    {
        const string json =
            "{\"outer\":{\"value\":1,\"value\":2}}";

        AssertDuplicateMemberRejected(
            () =>
                StrictJson.DeserializeObject<
                    Dictionary<string, object?>>(
                        json,
                        "Test object"),
            "value");
    }

    [Fact]
    public void LifestyleLoader_RejectsDuplicateIdentityMember()
    {
        const string json =
            """
            [
              {
                "id": "dnd5e2014.lifestyle.test",
                "id": "dnd5e2014.lifestyle.other",
                "name": "Test lifestyle",
                "dailyCost": {
                  "amount": {
                    "copperPieces": 100
                  },
                  "kind": "Exact"
                },
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

        AssertDuplicateMemberRejected(
            () =>
                LifestyleDefinitionLoader.LoadFromJson(
                    json),
            "id");
    }

    [Fact]
    public void FoodDrinkLoader_RejectsDuplicatePriceMember()
    {
        const string json =
            """
            [
              {
                "id": "dnd5e2014.food-drink.test",
                "name": "Test food",
                "cost": {
                  "copperPieces": 2,
                  "copperPieces": 3
                },
                "pricingUnit": "Loaf",
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

        AssertDuplicateMemberRejected(
            () =>
                FoodDrinkDefinitionLoader.LoadFromJson(
                    json),
            "copperPieces");
    }

    [Fact]
    public void MundaneServiceLoader_RejectsDuplicateEnumMember()
    {
        const string json =
            """
            [
              {
                "id": "dnd5e2014.mundane-service.test",
                "name": "Test service",
                "cost": {
                  "amount": {
                    "copperPieces": 100
                  },
                  "kind": "Exact"
                },
                "pricingUnit": "Flat",
                "pricingUnit": "Day",
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

        AssertDuplicateMemberRejected(
            () =>
                MundaneServiceDefinitionLoader.LoadFromJson(
                    json),
            "pricingUnit");
    }

    [Fact]
    public void LifestyleLoader_RejectsDuplicateAssociationMember()
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
                "specialRuleIds": [
                  "dnd5e2014.lifestyle-rule.test"
                ],
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

        AssertDuplicateMemberRejected(
            () =>
                LifestyleDefinitionLoader.LoadFromJson(
                    json),
            "specialRuleIds");
    }

    [Fact]
    public void HospitalityLoader_RejectsDuplicateProvenanceMember()
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
                "sources": [
                  {
                    "documentId": "dnd5e2014.source.phb-first-printing",
                    "page": 158,
                    "page": 159,
                    "section": "Food, Drink, and Lodging"
                  }
                ]
              }
            ]
            """;

        AssertDuplicateMemberRejected(
            () =>
                LifestyleHospitalityCostDefinitionLoader
                    .LoadFromJson(json),
            "page");
    }

    private static void AssertDuplicateMemberRejected(
        Action action,
        string propertyName)
    {
        InvalidDataException exception =
            Assert.Throws<InvalidDataException>(action);

        JsonException inner =
            Assert.IsType<JsonException>(
                exception.InnerException);

        Assert.Contains(
            $"Duplicate JSON property '{propertyName}'",
            inner.Message,
            StringComparison.Ordinal);
    }
}
