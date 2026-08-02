using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Expenses.Services;

namespace FiveEData.Tests;

public sealed class MundaneServiceImmutabilityTests
{
    [Fact]
    public void Definition_DefensivelySnapshotsCollections()
    {
        var ruleIds = new List<RuleId>
        {
            new("dnd5e2014.expense-rule.test")
        };

        var sources = new List<SourceReference>
        {
            new(
                new SourceDocumentId(
                    "dnd5e2014.source.phb-first-printing"),
                page: 159)
        };

        var definition = new MundaneServiceDefinition(
            new MundaneServiceId(
                "dnd5e2014.mundane-service.test"),
            "Test service",
            new ListedCost(
                new Money(100),
                ListedCostKind.Exact),
            ServicePricingUnit.Day,
            ruleIds,
            sources);

        ruleIds.Clear();
        sources.Clear();

        Assert.Single(definition.SpecialRuleIds);
        Assert.Single(definition.Sources);
    }

    [Fact]
    public void Definition_CollectionsRejectMutation()
    {
        var definition = new MundaneServiceDefinition(
            new MundaneServiceId(
                "dnd5e2014.mundane-service.test"),
            "Test service",
            new ListedCost(
                new Money(100),
                ListedCostKind.Exact),
            ServicePricingUnit.Day,
            [
                new RuleId(
                    "dnd5e2014.expense-rule.test")
            ],
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 159)
            ]);

        IList<RuleId> ruleIds =
            Assert.IsAssignableFrom<IList<RuleId>>(
                definition.SpecialRuleIds);

        IList<SourceReference> sources =
            Assert.IsAssignableFrom<IList<SourceReference>>(
                definition.Sources);

        Assert.Throws<NotSupportedException>(
            () => ruleIds.Clear());

        Assert.Throws<NotSupportedException>(
            () => sources.Clear());
    }
}
