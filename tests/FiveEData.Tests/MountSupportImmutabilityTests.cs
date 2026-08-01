using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment.MountSupport;

namespace FiveEData.Tests;

public sealed class MountSupportImmutabilityTests
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

        var definition = new MountSupportDefinition(
            new MountSupportId("dnd5e2014.mount-support.test"),
            "Test mount support",
            new Money(100),
            listedWeight: new Weight(1),
            specialRuleIds: rules,
            sources: sources);

        rules.Add(new RuleId("dnd5e2014.mount-support-rule.mutated"));
        sources.Clear();

        Assert.Empty(definition.SpecialRuleIds);
        Assert.Single(definition.Sources);
    }
}
