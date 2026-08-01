using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Expenses.FoodAndLodging;
using FiveEData.Rules.Expenses.Lifestyles;

namespace FiveEData.Tests;

public sealed class
    LifestyleHospitalityCostImmutabilityTests
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
                page: 158)
        };

        var definition =
            new LifestyleHospitalityCostDefinition(
                new LifestyleId(
                    "dnd5e2014.lifestyle.modest"),
                new Money(50),
                new Money(30),
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
        var definition =
            new LifestyleHospitalityCostDefinition(
                new LifestyleId(
                    "dnd5e2014.lifestyle.modest"),
                new Money(50),
                new Money(30),
                [
                    new RuleId(
                        "dnd5e2014.expense-rule.test")
                ],
                [
                    new SourceReference(
                        new SourceDocumentId(
                            "dnd5e2014.source.phb-first-printing"),
                        page: 158)
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
