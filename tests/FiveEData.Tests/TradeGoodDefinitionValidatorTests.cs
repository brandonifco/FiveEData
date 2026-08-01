using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment.TradeGoods;

namespace FiveEData.Tests;

public sealed class TradeGoodDefinitionValidatorTests
{
    [Fact]
    public void ValidDefinition_HasNoErrors()
    {
        Assert.Empty(
            TradeGoodDefinitionValidator.Validate(
                Create(
                    new TradeGoodId(
                        "dnd5e2014.trade-good.test"))));
    }

    [Fact]
    public void DefaultId_IsRejected()
    {
        TradeGoodDefinition definition = Create(default);

        Assert.Contains(
            TradeGoodDefinitionValidator.Validate(definition),
            error => error.Contains("ID", StringComparison.Ordinal));
    }

    [Fact]
    public void ZeroMarketValue_IsRejected()
    {
        TradeGoodDefinition definition = Create(
            new TradeGoodId("dnd5e2014.trade-good.test"),
            marketValue: new Money(0));

        Assert.Contains(
            TradeGoodDefinitionValidator.Validate(definition),
            error => error.Contains(
                "market value",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void DefaultPricingBasis_IsRejected()
    {
        TradeGoodDefinition definition = new(
            new TradeGoodId("dnd5e2014.trade-good.test"),
            "Test trade good",
            new Money(100),
            pricingBasis: default,
            specialRuleIds: [],
            sources:
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 157)
            ]);

        IReadOnlyList<string> errors =
            TradeGoodDefinitionValidator.Validate(definition);

        Assert.Contains(
            errors,
            error => error.Contains(
                "quantity",
                StringComparison.OrdinalIgnoreCase));
        Assert.Contains(
            errors,
            error => error.Contains(
                "unit",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void DuplicateSpecialRuleIds_AreRejected()
    {
        var ruleId =
            new RuleId("dnd5e2014.trade-good-rule.test");

        TradeGoodDefinition definition = Create(
            new TradeGoodId("dnd5e2014.trade-good.test"),
            specialRuleIds: [ruleId, ruleId]);

        Assert.Contains(
            TradeGoodDefinitionValidator.Validate(definition),
            error => error.Contains(
                "duplicated",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void DefaultSpecialRuleId_IsRejected()
    {
        TradeGoodDefinition definition = Create(
            new TradeGoodId("dnd5e2014.trade-good.test"),
            specialRuleIds: [default]);

        Assert.Contains(
            TradeGoodDefinitionValidator.Validate(definition),
            error => error.Contains(
                "rule ID",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void NoSources_IsRejected()
    {
        TradeGoodDefinition definition = Create(
            new TradeGoodId("dnd5e2014.trade-good.test"),
            sources: []);

        Assert.Contains(
            TradeGoodDefinitionValidator.Validate(definition),
            error => error.Contains(
                "source",
                StringComparison.OrdinalIgnoreCase));
    }

    private static TradeGoodDefinition Create(
        TradeGoodId id,
        Money? marketValue = null,
        TradeGoodPricingBasis? pricingBasis = null,
        IEnumerable<RuleId>? specialRuleIds = null,
        IEnumerable<SourceReference>? sources = null)
    {
        return new TradeGoodDefinition(
            id,
            "Test trade good",
            marketValue ?? new Money(100),
            pricingBasis ??
                new TradeGoodPricingBasis(1, TradeGoodUnit.Pound),
            specialRuleIds ?? [],
            sources ??
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 157)
            ]);
    }
}
