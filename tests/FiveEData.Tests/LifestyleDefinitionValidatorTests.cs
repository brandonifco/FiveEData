using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Expenses.Lifestyles;

namespace FiveEData.Tests;

public sealed class LifestyleDefinitionValidatorTests
{
    [Fact]
    public void ValidDefinition_HasNoErrors()
    {
        LifestyleDefinition definition = Create();

        Assert.Empty(
            LifestyleDefinitionValidator.Validate(definition));
    }

    [Fact]
    public void MissingDailyCost_IsAllowed()
    {
        LifestyleDefinition definition = Create(
            hasDailyCost: false);

        Assert.Null(definition.DailyCost);
        Assert.Empty(
            LifestyleDefinitionValidator.Validate(definition));
    }

    [Fact]
    public void DefaultId_IsRejected()
    {
        LifestyleDefinition definition = Create(
            id: default(LifestyleId));

        Assert.Contains(
            LifestyleDefinitionValidator.Validate(definition),
            error => error.Contains(
                "ID must not be empty",
                StringComparison.Ordinal));
    }

    [Fact]
    public void DefaultDailyCost_WhenSpecified_IsRejected()
    {
        LifestyleDefinition definition = Create(
            dailyCost: default(ListedCost));

        IReadOnlyList<string> errors =
            LifestyleDefinitionValidator.Validate(definition);

        Assert.Contains(
            errors,
            error => error.Contains(
                "greater than zero",
                StringComparison.Ordinal));
        Assert.Contains(
            errors,
            error => error.Contains(
                "kind must be defined",
                StringComparison.Ordinal));
    }

    [Fact]
    public void DuplicateRuleIds_AreRejected()
    {
        var ruleId = new RuleId(
            "dnd5e2014.lifestyle-rule.test");

        LifestyleDefinition definition = Create(
            specialRuleIds: [ruleId, ruleId]);

        Assert.Contains(
            LifestyleDefinitionValidator.Validate(definition),
            error => error.Contains(
                "duplicated",
                StringComparison.Ordinal));
    }

    [Fact]
    public void MissingSources_AreRejected()
    {
        LifestyleDefinition definition = Create(sources: []);

        Assert.Contains(
            LifestyleDefinitionValidator.Validate(definition),
            error => error.Contains(
                "at least one source",
                StringComparison.Ordinal));
    }

    private static LifestyleDefinition Create(
        LifestyleId? id = null,
        ListedCost? dailyCost = null,
        bool hasDailyCost = true,
        IEnumerable<RuleId>? specialRuleIds = null,
        IEnumerable<SourceReference>? sources = null)
    {
        ListedCost? resolvedDailyCost = hasDailyCost
            ? dailyCost ??
              new ListedCost(
                  new Money(100),
                  ListedCostKind.Exact)
            : null;

        return new LifestyleDefinition(
            id ?? new LifestyleId(
                "dnd5e2014.lifestyle.test"),
            "Test lifestyle",
            resolvedDailyCost,
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
