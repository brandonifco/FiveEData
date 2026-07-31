using FiveEData.Rules.Common;
using FiveEData.Rules.Equipment;

namespace FiveEData.Tests;

public sealed class ArmorUsageRulesetTests
{
    [Fact]
    public void Ruleset_ExposesCanonicalArmorUsageRules()
    {
        Dnd5e2014Ruleset ruleset = Dnd5e2014Ruleset.Instance;

        Assert.Equal(10, ruleset.ArmorUsage.InsufficientStrengthSpeedReduction.Feet);
        Assert.Equal(1, ruleset.ArmorUsage.ShieldHandsRequired);
        Assert.Equal(1, ruleset.ArmorUsage.MaximumBenefitingShields);
    }

    [Fact]
    public void Ruleset_ExposesFirstPrintingArmorProficiencyConsequences()
    {
        var consequences =
            Dnd5e2014Ruleset.Instance.ArmorUsage.ArmorProficiencyConsequences;

        Assert.True(consequences.DisadvantageOnStrengthOrDexterityAbilityChecks);
        Assert.True(consequences.DisadvantageOnStrengthOrDexteritySavingThrows);
        Assert.True(consequences.DisadvantageOnStrengthOrDexterityAttackRolls);
        Assert.True(consequences.PreventsSpellcasting);
    }

    [Fact]
    public void EveryArmorUsageRuleReference_Resolves()
    {
        Dnd5e2014Ruleset ruleset = Dnd5e2014Ruleset.Instance;

        foreach (RuleId id in ruleset.ArmorUsage.ReferencedRuleIds)
        {
            Assert.True(ruleset.Rules.TryGet(id, out var rule));
            Assert.NotNull(rule);
        }
    }

    [Fact]
    public void ShieldChangeTiming_UsesActionsRatherThanMinutes()
    {
        var timing = Dnd5e2014Ruleset.Instance.ArmorUsage.ShieldChangeTiming;

        Assert.Equal(1, timing.Don.Amount);
        Assert.Equal(EquipmentChangeTimeUnit.Action, timing.Don.Unit);
        Assert.Equal(1, timing.Doff.Amount);
        Assert.Equal(EquipmentChangeTimeUnit.Action, timing.Doff.Unit);
    }
}
