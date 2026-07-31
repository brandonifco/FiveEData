using FiveEData.Rules.Common;
using FiveEData.Rules.Equipment.AdventuringGear;

namespace FiveEData.Tests;

public sealed class Phase6AdventuringGearCoverageTests
{
    [Fact]
    public void FirstPrintingAdventuringGearTable_IsCompleteAcrossDedicatedCatalogs()
    {
        Dnd5e2014Ruleset ruleset = Dnd5e2014Ruleset.Instance;

        Assert.Equal(95, ruleset.AdventuringGear.Count);
        Assert.Equal(4, ruleset.Ammunition.Count);
        Assert.Equal(
            99,
            ruleset.AdventuringGear.Count + ruleset.Ammunition.Count);

        string[] tableRows = ruleset.AdventuringGear.All
            .Select(definition => definition.Name)
            .Concat(
                ruleset.Ammunition.All.Select(
                    definition => $"{definition.Name} ({definition.BundleQuantity})"))
            .ToArray();

        Assert.Equal(99, tableRows.Distinct(StringComparer.Ordinal).Count());
        string[] expectedAmmunitionRows =
        [
            "Arrows (20)",
            "Blowgun needles (50)",
            "Crossbow bolts (20)",
            "Sling bullets (20)"
        ];

        Assert.Equal(
            expectedAmmunitionRows,
            ruleset.Ammunition.All
                .Select(
                    definition => $"{definition.Name} ({definition.BundleQuantity})")
                .OrderBy(value => value, StringComparer.Ordinal)
                .ToArray());

        Assert.All(
            ruleset.AdventuringGear.All,
            definition => Assert.Equal(150, Assert.Single(definition.Sources).Page));
        Assert.All(
            ruleset.Ammunition.All,
            definition => Assert.Equal(150, Assert.Single(definition.Sources).Page));
    }

    [Fact]
    public void AdventuringGearDescriptionRules_HaveBidirectionalCoverage()
    {
        Dnd5e2014Ruleset ruleset = Dnd5e2014Ruleset.Instance;
        const string prefix = "dnd5e2014.adventuring-gear-rule.";

        RuleId[] canonicalRuleIds = ruleset.Rules.All
            .Where(
                rule => rule.Id.Value.StartsWith(
                    prefix,
                    StringComparison.Ordinal))
            .Select(rule => rule.Id)
            .OrderBy(ruleId => ruleId.Value, StringComparer.Ordinal)
            .ToArray();

        RuleId[] references = ruleset.AdventuringGear.All
            .SelectMany(definition => definition.SpecialRuleIds)
            .ToArray();

        RuleId[] distinctReferences = references
            .Distinct()
            .OrderBy(ruleId => ruleId.Value, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(42, canonicalRuleIds.Length);
        Assert.Equal(52, references.Length);
        Assert.Equal(42, distinctReferences.Length);
        Assert.Equal(canonicalRuleIds, distinctReferences);
    }

    [Fact]
    public void ContainerCapacitySurface_ReferencesUniqueCanonicalGear()
    {
        Dnd5e2014Ruleset ruleset = Dnd5e2014Ruleset.Instance;

        Assert.Equal(13, ruleset.ContainerCapacities.Count);
        Assert.Equal(
            13,
            ruleset.ContainerCapacities.All
                .Select(definition => definition.AdventuringGearId)
                .Distinct()
                .Count());

        foreach (ContainerCapacityDefinition capacity in ruleset.ContainerCapacities.All)
        {
            Assert.True(
                ruleset.AdventuringGear.TryGet(
                    capacity.AdventuringGearId,
                    out AdventuringGearDefinition? gear));
            Assert.NotNull(gear);
        }
    }
}
