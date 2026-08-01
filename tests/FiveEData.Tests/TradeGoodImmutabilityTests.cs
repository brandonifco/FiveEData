using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment.TradeGoods;

namespace FiveEData.Tests;

public sealed class TradeGoodImmutabilityTests
{
    [Fact]
    public void Definition_DefensivelySnapshotsCollectionInputs()
    {
        var rules = new List<RuleId>();
        var sources = new List<SourceReference>
        {
            new(
                new SourceDocumentId(
                    "dnd5e2014.source.phb-first-printing"),
                page: 157)
        };

        var definition = new TradeGoodDefinition(
            new TradeGoodId("dnd5e2014.trade-good.test"),
            "Test trade good",
            new Money(100),
            new TradeGoodPricingBasis(1, TradeGoodUnit.Pound),
            specialRuleIds: rules,
            sources: sources);

        rules.Add(new RuleId("dnd5e2014.trade-good-rule.mutated"));
        sources.Clear();

        Assert.Empty(definition.SpecialRuleIds);
        Assert.Single(definition.Sources);
    }
}
