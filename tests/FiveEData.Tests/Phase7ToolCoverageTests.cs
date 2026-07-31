using FiveEData.Rules.Common;
using FiveEData.Rules.Equipment.Tools;

namespace FiveEData.Tests;

public sealed class Phase7ToolCoverageTests
{
    [Fact]
    public void CanonicalToolSurface_IsCompleteAndPartitionedBySourceFamily()
    {
        Dnd5e2014Ruleset ruleset = Dnd5e2014Ruleset.Instance;

        Assert.Equal(37, ruleset.Tools.Count);
        Assert.Equal(3, ruleset.ToolFamilies.Count);

        ToolFamilyId artisan =
            new("dnd5e2014.tool-family.artisans-tools");
        ToolFamilyId gaming =
            new("dnd5e2014.tool-family.gaming-set");
        ToolFamilyId instrument =
            new("dnd5e2014.tool-family.musical-instrument");

        Assert.Equal(
            17,
            ruleset.Tools.All.Count(tool => tool.FamilyId == artisan));
        Assert.Equal(
            4,
            ruleset.Tools.All.Count(tool => tool.FamilyId == gaming));
        Assert.Equal(
            10,
            ruleset.Tools.All.Count(tool => tool.FamilyId == instrument));
        Assert.Equal(
            6,
            ruleset.Tools.All.Count(tool => tool.FamilyId is null));

        Assert.DoesNotContain(
            ruleset.Tools.All,
            tool => tool.Name.Contains(
                "Vehicle",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void ToolRuleReferences_HaveCompleteBidirectionalCoverageAndProvenance()
    {
        Dnd5e2014Ruleset ruleset = Dnd5e2014Ruleset.Instance;

        RuleDefinition[] toolRules = ruleset.Rules.All
            .Where(rule => rule.Id.Value.StartsWith(
                "dnd5e2014.tool-rule.",
                StringComparison.Ordinal))
            .ToArray();

        Assert.Equal(10, toolRules.Length);

        RuleId[] toolReferences = ruleset.Tools.All
            .SelectMany(tool => tool.SpecialRuleIds)
            .ToArray();
        RuleId[] familyReferences = ruleset.ToolFamilies.All
            .SelectMany(family => family.SpecialRuleIds)
            .ToArray();

        Assert.Equal(43, toolReferences.Length);
        Assert.Equal(3, familyReferences.Length);

        HashSet<RuleId> referenced =
            toolReferences.Concat(familyReferences).ToHashSet();
        HashSet<RuleId> defined =
            toolRules.Select(rule => rule.Id).ToHashSet();

        Assert.True(defined.SetEquals(referenced));

        foreach (RuleDefinition rule in toolRules)
        {
            Assert.Single(rule.Sources);
            Assert.Equal(154, rule.Sources[0].Page);
            Assert.Equal(
                "dnd5e2014.source.phb-first-printing",
                rule.Sources[0].DocumentId.Value);
        }
    }
}
