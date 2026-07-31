using FiveEData.Rules.Common;

namespace FiveEData.Tests;

public sealed class RuleCatalogTests
{
    [Fact]
    public void EmbeddedRuleCatalog_ResolvesCurrentCanonicalRules()
    {
        Dnd5e2014Ruleset ruleset = Dnd5e2014Ruleset.Instance;

        Assert.Equal(7, ruleset.Rules.Count);

        RuleDefinition lance =
            ruleset.Rules.Get(
                new RuleId("dnd5e2014.weapon-rule.lance"));

        RuleDefinition net =
            ruleset.Rules.Get(
                new RuleId("dnd5e2014.weapon-rule.net"));

        RuleDefinition armorProficiency =
            ruleset.Rules.Get(
                new RuleId("dnd5e2014.armor-rule.proficiency"));

        RuleDefinition donDoff =
            ruleset.Rules.Get(
                new RuleId("dnd5e2014.armor-rule.don-doff"));

        Assert.Equal("Lance special weapon rule", lance.Name);
        Assert.Equal("Net special weapon rule", net.Name);
        Assert.Equal("Armor proficiency consequences", armorProficiency.Name);
        Assert.Equal("Donning and doffing armor", donDoff.Name);
    }

    [Fact]
    public void EveryWeaponSpecialRuleId_Resolves()
    {
        Dnd5e2014Ruleset ruleset = Dnd5e2014Ruleset.Instance;

        foreach (var weapon in ruleset.Weapons.All)
        {
            foreach (RuleId ruleId in weapon.SpecialRuleIds)
            {
                Assert.True(
                    ruleset.Rules.TryGet(
                        ruleId,
                        out RuleDefinition? definition));
                Assert.NotNull(definition);
            }
        }
    }
}
