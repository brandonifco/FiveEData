using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Expenses.Services;

namespace FiveEData.Tests;

public sealed class MundaneServiceDefinitionValidatorTests
{
    [Fact]
    public void ValidDefinition_HasNoErrors()
    {
        Assert.Empty(
            MundaneServiceDefinitionValidator.Validate(
                Create()));
    }

    [Fact]
    public void DefaultId_IsRejected()
    {
        var definition = new MundaneServiceDefinition(
            default,
            "Test service",
            new ListedCost(
                new Money(100),
                ListedCostKind.Exact),
            ServicePricingUnit.Day,
            specialRuleIds: [],
            sources:
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 159)
            ]);

        Assert.Contains(
            MundaneServiceDefinitionValidator.Validate(
                definition),
            error => error.Contains(
                "ID must not be empty",
                StringComparison.Ordinal));
    }

    [Fact]
    public void DefaultCost_IsRejected()
    {
        MundaneServiceDefinition definition =
            Create(cost: default(ListedCost));

        IReadOnlyList<string> errors =
            MundaneServiceDefinitionValidator.Validate(
                definition);

        Assert.Contains(
            errors,
            error => error.Contains(
                "greater than zero",
                StringComparison.Ordinal));

        Assert.Contains(
            errors,
            error => error.Contains(
                "cost kind must be defined",
                StringComparison.Ordinal));
    }

    [Fact]
    public void UndefinedPricingUnit_IsRejected()
    {
        MundaneServiceDefinition definition =
            Create(pricingUnit: (ServicePricingUnit)999);

        Assert.Contains(
            MundaneServiceDefinitionValidator.Validate(
                definition),
            error => error.Contains(
                "pricing unit must be defined",
                StringComparison.Ordinal));
    }

    [Fact]
    public void DuplicateRuleIds_AreRejected()
    {
        var ruleId = new RuleId(
            "dnd5e2014.expense-rule.test");

        MundaneServiceDefinition definition =
            Create(specialRuleIds: [ruleId, ruleId]);

        Assert.Contains(
            MundaneServiceDefinitionValidator.Validate(
                definition),
            error => error.Contains(
                "duplicated",
                StringComparison.Ordinal));
    }

    [Fact]
    public void EmptyRuleId_IsRejected()
    {
        MundaneServiceDefinition definition =
            Create(specialRuleIds: [default]);

        Assert.Contains(
            MundaneServiceDefinitionValidator.Validate(
                definition),
            error => error.Contains(
                "rule ID must not be empty",
                StringComparison.Ordinal));
    }

    [Fact]
    public void MissingSources_AreRejected()
    {
        MundaneServiceDefinition definition =
            Create(sources: []);

        Assert.Contains(
            MundaneServiceDefinitionValidator.Validate(
                definition),
            error => error.Contains(
                "at least one source",
                StringComparison.Ordinal));
    }

    private static MundaneServiceDefinition Create(
        MundaneServiceId? id = null,
        ListedCost? cost = null,
        ServicePricingUnit pricingUnit =
            ServicePricingUnit.Day,
        IEnumerable<RuleId>? specialRuleIds = null,
        IEnumerable<SourceReference>? sources = null)
    {
        return new MundaneServiceDefinition(
            id ?? new MundaneServiceId(
                "dnd5e2014.mundane-service.test"),
            "Test service",
            cost ??
            new ListedCost(
                new Money(100),
                ListedCostKind.Exact),
            pricingUnit,
            specialRuleIds ?? [],
            sources ??
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 159)
            ]);
    }
}
