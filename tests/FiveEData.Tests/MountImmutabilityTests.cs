using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment.Mounts;

namespace FiveEData.Tests;

public sealed class MountImmutabilityTests
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
                page: 155)
        };

        var mount = new MountDefinition(
            new MountId("dnd5e2014.mount.test"),
            "Test mount",
            new Money(100),
            new Distance(40),
            new Weight(100),
            rules,
            sources);

        rules.Add(new RuleId("dnd5e2014.mount-rule.mutated"));
        sources.Clear();

        Assert.Empty(mount.SpecialRuleIds);
        Assert.Single(mount.Sources);
    }
}
