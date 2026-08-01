using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Expenses.Lifestyles;

namespace FiveEData.Tests;

public sealed class LifestyleImmutabilityTests
{
    [Fact]
    public void Definition_DefensivelySnapshotsRuleAndSourceInputs()
    {
        var ruleIds = new List<RuleId>
        {
            new("dnd5e2014.lifestyle-rule.test")
        };

        var sources = new List<SourceReference>
        {
            new(
                new SourceDocumentId(
                    "dnd5e2014.source.phb-first-printing"),
                page: 157)
        };

        var definition = new LifestyleDefinition(
            new LifestyleId(
                "dnd5e2014.lifestyle.test"),
            "Test lifestyle",
            new ListedCost(
                new Money(100),
                ListedCostKind.Exact),
            ruleIds,
            sources);

        ruleIds.Clear();
        sources.Clear();

        Assert.Single(definition.SpecialRuleIds);
        Assert.Single(definition.Sources);
    }

    [Fact]
    public void Definition_CollectionsCannotBeMutatedThroughPublicReferences()
    {
        var definition = new LifestyleDefinition(
            new LifestyleId(
                "dnd5e2014.lifestyle.test"),
            "Test lifestyle",
            new ListedCost(
                new Money(100),
                ListedCostKind.Exact),
            [
                new RuleId(
                    "dnd5e2014.lifestyle-rule.test")
            ],
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 157)
            ]);

        IList<RuleId> ruleIds =
            Assert.IsAssignableFrom<IList<RuleId>>(
                definition.SpecialRuleIds);

        IList<SourceReference> sources =
            Assert.IsAssignableFrom<IList<SourceReference>>(
                definition.Sources);

        Assert.Throws<NotSupportedException>(
            () => ruleIds.Add(
                new RuleId(
                    "dnd5e2014.lifestyle-rule.other")));

        Assert.Throws<NotSupportedException>(
            () => sources.Clear());
    }
}
