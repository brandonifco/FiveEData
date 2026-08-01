using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Expenses.FoodAndLodging;
using FiveEData.Rules.Expenses.Lifestyles;

namespace FiveEData.Tests;

public sealed class
    LifestyleHospitalityCostDefinitionValidatorTests
{
    [Fact]
    public void ValidDefinition_HasNoErrors()
    {
        Assert.Empty(
            LifestyleHospitalityCostDefinitionValidator
                .Validate(Create()));
    }

    [Fact]
    public void DefaultLifestyleId_IsRejected()
    {
        LifestyleHospitalityCostDefinition definition =
            new(
                default,
                new Money(50),
                new Money(30),
                specialRuleIds: [],
                sources: [CreateSource()]);

        Assert.Contains(
            LifestyleHospitalityCostDefinitionValidator
                .Validate(definition),
            error => error.Contains(
                "lifestyle ID must not be empty",
                StringComparison.Ordinal));
    }

    [Fact]
    public void NonpositiveInnStayCost_IsRejected()
    {
        LifestyleHospitalityCostDefinition definition =
            Create(innStayCost: new Money(0));

        Assert.Contains(
            LifestyleHospitalityCostDefinitionValidator
                .Validate(definition),
            error => error.Contains(
                "inn-stay cost",
                StringComparison.Ordinal));
    }

    [Fact]
    public void NonpositiveMealsCost_IsRejected()
    {
        LifestyleHospitalityCostDefinition definition =
            Create(mealsCost: new Money(0));

        Assert.Contains(
            LifestyleHospitalityCostDefinitionValidator
                .Validate(definition),
            error => error.Contains(
                "meals cost",
                StringComparison.Ordinal));
    }

    [Fact]
    public void DuplicateRuleIds_AreRejected()
    {
        var ruleId = new RuleId(
            "dnd5e2014.expense-rule.test");

        LifestyleHospitalityCostDefinition definition =
            Create(specialRuleIds: [ruleId, ruleId]);

        Assert.Contains(
            LifestyleHospitalityCostDefinitionValidator
                .Validate(definition),
            error => error.Contains(
                "duplicated",
                StringComparison.Ordinal));
    }

    [Fact]
    public void MissingSources_AreRejected()
    {
        LifestyleHospitalityCostDefinition definition =
            Create(sources: []);

        Assert.Contains(
            LifestyleHospitalityCostDefinitionValidator
                .Validate(definition),
            error => error.Contains(
                "at least one source",
                StringComparison.Ordinal));
    }

    private static LifestyleHospitalityCostDefinition Create(
        Money? innStayCost = null,
        Money? mealsCost = null,
        IEnumerable<RuleId>? specialRuleIds = null,
        IEnumerable<SourceReference>? sources = null)
    {
        return new LifestyleHospitalityCostDefinition(
            new LifestyleId(
                "dnd5e2014.lifestyle.modest"),
            innStayCost ?? new Money(50),
            mealsCost ?? new Money(30),
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
