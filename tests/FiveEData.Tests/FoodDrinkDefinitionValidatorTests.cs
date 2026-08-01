using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Expenses.FoodAndLodging;

namespace FiveEData.Tests;

public sealed class FoodDrinkDefinitionValidatorTests
{
    [Fact]
    public void ValidDefinition_HasNoErrors()
    {
        Assert.Empty(
            FoodDrinkDefinitionValidator.Validate(Create()));
    }

    [Fact]
    public void DefaultId_IsRejected()
    {
        FoodDrinkDefinition definition = new(
            default,
            "Test food",
            new Money(10),
            FoodDrinkPricingUnit.Loaf,
            specialRuleIds: [],
            sources: [CreateSource()]);

        Assert.Contains(
            FoodDrinkDefinitionValidator.Validate(definition),
            error => error.Contains(
                "ID must not be empty",
                StringComparison.Ordinal));
    }

    [Fact]
    public void NonpositiveCost_IsRejected()
    {
        FoodDrinkDefinition definition = Create(
            cost: new Money(0));

        Assert.Contains(
            FoodDrinkDefinitionValidator.Validate(definition),
            error => error.Contains(
                "greater than zero",
                StringComparison.Ordinal));
    }

    [Fact]
    public void UndefinedPricingUnit_IsRejected()
    {
        FoodDrinkDefinition definition = Create(
            pricingUnit: default);

        Assert.Contains(
            FoodDrinkDefinitionValidator.Validate(definition),
            error => error.Contains(
                "pricing unit must be defined",
                StringComparison.Ordinal));
    }

    [Fact]
    public void DuplicateRuleIds_AreRejected()
    {
        var ruleId = new RuleId(
            "dnd5e2014.expense-rule.test");

        FoodDrinkDefinition definition = Create(
            specialRuleIds: [ruleId, ruleId]);

        Assert.Contains(
            FoodDrinkDefinitionValidator.Validate(definition),
            error => error.Contains(
                "duplicated",
                StringComparison.Ordinal));
    }

    [Fact]
    public void MissingSources_AreRejected()
    {
        FoodDrinkDefinition definition = Create(sources: []);

        Assert.Contains(
            FoodDrinkDefinitionValidator.Validate(definition),
            error => error.Contains(
                "at least one source",
                StringComparison.Ordinal));
    }

    private static FoodDrinkDefinition Create(
        Money? cost = null,
        FoodDrinkPricingUnit pricingUnit =
            FoodDrinkPricingUnit.Loaf,
        IEnumerable<RuleId>? specialRuleIds = null,
        IEnumerable<SourceReference>? sources = null)
    {
        return new FoodDrinkDefinition(
            new FoodDrinkId(
                "dnd5e2014.food-drink.test"),
            "Test food",
            cost ?? new Money(10),
            pricingUnit,
            specialRuleIds ?? [],
            sources ?? [CreateSource()]);
    }

    private static SourceReference CreateSource()
    {
        return new SourceReference(
            new SourceDocumentId(
                "dnd5e2014.source.phb-first-printing"),
            page: 158);
    }
}
