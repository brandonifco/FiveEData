using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment.Tools;

namespace FiveEData.Tests;

public sealed class ToolImmutabilityTests
{
    [Fact]
    public void Definitions_DefensivelySnapshotCollectionInputs()
    {
        var rules = new List<RuleId>();
        var sources = new List<SourceReference>
        {
            new(
                new SourceDocumentId(
                    "dnd5e2014.source.phb-first-printing"),
                page: 154)
        };

        var tool = new ToolDefinition(
            new ToolId("dnd5e2014.tool.test"),
            "Test",
            new Money(100),
            weight: null,
            familyId: null,
            rules,
            sources);

        var family = new ToolFamilyDefinition(
            new ToolFamilyId("dnd5e2014.tool-family.test"),
            "Test family",
            rules,
            sources);

        rules.Add(new RuleId("dnd5e2014.rule.mutated"));
        sources.Clear();

        Assert.Empty(tool.SpecialRuleIds);
        Assert.Single(tool.Sources);
        Assert.Empty(family.SpecialRuleIds);
        Assert.Single(family.Sources);
    }
}
