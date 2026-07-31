using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment.AdventuringGear;

namespace FiveEData.Tests;

public sealed class AdventuringGearImmutabilityTests
{
    [Fact]
    public void Definition_DefensivelySnapshotsCollections()
    {
        var ruleIds = new List<RuleId>
        {
            new("dnd5e2014.adventuring-gear-rule.original")
        };
        var sources = new List<SourceReference>
        {
            new(
                new SourceDocumentId(
                    "dnd5e2014.source.phb-first-printing"),
                page: 150)
        };

        AdventuringGearDefinition definition = new(
            new AdventuringGearId("dnd5e2014.adventuring-gear.test"),
            "Test gear",
            new Money(100),
            listedWeight: null,
            ruleIds,
            sources);

        ruleIds.Add(new RuleId("dnd5e2014.adventuring-gear-rule.added"));
        sources.Add(
            new SourceReference(
                new SourceDocumentId("dnd5e2014.source.other"),
                page: 1));

        Assert.Single(definition.SpecialRuleIds);
        Assert.Single(definition.Sources);
    }

    [Fact]
    public void Definition_ExposesNoPublicSetters()
    {
        Assert.All(
            typeof(AdventuringGearDefinition).GetProperties(),
            property => Assert.Null(property.SetMethod));
    }
}
