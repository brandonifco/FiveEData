using FiveEData.Rules.Common;

namespace FiveEData.Tests;

public sealed class RuleCatalogTests
{
    [Fact]
    public void EmbeddedRuleCatalog_ResolvesCurrentCanonicalRules()
    {
        Dnd5e2014Ruleset ruleset = Dnd5e2014Ruleset.Instance;

        Assert.Equal(173, ruleset.Rules.Count);

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

    [Fact]
    public void EveryAdventuringGearSpecialRuleId_Resolves()
    {
        Dnd5e2014Ruleset ruleset = Dnd5e2014Ruleset.Instance;

        foreach (var item in ruleset.AdventuringGear.All)
        {
            foreach (RuleId ruleId in item.SpecialRuleIds)
            {
                Assert.True(
                    ruleset.Rules.TryGet(
                        ruleId,
                        out RuleDefinition? definition));
                Assert.NotNull(definition);
            }
        }
    }

    [Fact]
    public void AdventuringGearRules_PreserveFirstPrintingDescriptionProvenance()
    {
        Dnd5e2014Ruleset ruleset = Dnd5e2014Ruleset.Instance;

        RuleDefinition acid = ruleset.Rules.Get(
            new RuleId("dnd5e2014.adventuring-gear-rule.acid"));
        RuleDefinition alchemistsFire = ruleset.Rules.Get(
            new RuleId("dnd5e2014.adventuring-gear-rule.alchemists-fire"));
        RuleDefinition torch = ruleset.Rules.Get(
            new RuleId("dnd5e2014.adventuring-gear-rule.torch"));

        Assert.Equal(148, Assert.Single(acid.Sources).Page);
        Assert.Equal(
            new int?[] { 148, 151 },
            alchemistsFire.Sources
                .Select(source => source.Page)
                .ToArray());
        Assert.Equal(153, Assert.Single(torch.Sources).Page);
    }

    [Fact]
    public void EveryToolAndToolFamilySpecialRuleId_Resolves()
    {
        Dnd5e2014Ruleset ruleset = Dnd5e2014Ruleset.Instance;

        foreach (var tool in ruleset.Tools.All)
        {
            foreach (RuleId ruleId in tool.SpecialRuleIds)
            {
                Assert.True(
                    ruleset.Rules.TryGet(
                        ruleId,
                        out RuleDefinition? definition));
                Assert.NotNull(definition);
            }
        }

        foreach (var family in ruleset.ToolFamilies.All)
        {
            foreach (RuleId ruleId in family.SpecialRuleIds)
            {
                Assert.True(
                    ruleset.Rules.TryGet(
                        ruleId,
                        out RuleDefinition? definition));
                Assert.NotNull(definition);
            }
        }
    }

    [Fact]
    public void ToolRules_PreserveFirstPrintingDescriptionProvenance()
    {
        Dnd5e2014Ruleset ruleset = Dnd5e2014Ruleset.Instance;

        string[] ruleIds =
        [
            "dnd5e2014.tool-rule.proficiency",
            "dnd5e2014.tool-rule.artisans-tools",
            "dnd5e2014.tool-rule.disguise-kit",
            "dnd5e2014.tool-rule.forgery-kit",
            "dnd5e2014.tool-rule.gaming-set",
            "dnd5e2014.tool-rule.herbalism-kit",
            "dnd5e2014.tool-rule.musical-instrument",
            "dnd5e2014.tool-rule.navigators-tools",
            "dnd5e2014.tool-rule.poisoners-kit",
            "dnd5e2014.tool-rule.thieves-tools"
        ];

        foreach (string value in ruleIds)
        {
            RuleDefinition definition =
                ruleset.Rules.Get(new RuleId(value));
            var source = Assert.Single(definition.Sources);
            Assert.Equal(154, source.Page);
            Assert.StartsWith("Chapter 5: Equipment — Tools", source.Section);
        }
    }

    [Fact]
    public void EveryMountVehicleAndMountSupportSpecialRuleId_Resolves()
    {
        Dnd5e2014Ruleset ruleset = Dnd5e2014Ruleset.Instance;

        IEnumerable<RuleId> ids =
            ruleset.Mounts.All.SelectMany(item => item.SpecialRuleIds)
                .Concat(
                    ruleset.Vehicles.All.SelectMany(
                        item => item.SpecialRuleIds))
                .Concat(
                    ruleset.MountSupport.All.SelectMany(
                        item => item.SpecialRuleIds));

        foreach (RuleId ruleId in ids)
        {
            Assert.True(
                ruleset.Rules.TryGet(
                    ruleId,
                    out RuleDefinition? definition));
            Assert.NotNull(definition);
        }
    }

    [Fact]
    public void MountVehicleRules_PreserveFirstPrintingProvenance()
    {
        Dnd5e2014Ruleset ruleset = Dnd5e2014Ruleset.Instance;

        string[] ruleIds =
        [
            "dnd5e2014.mount-vehicle-rule.drawn-vehicle-pulling-capacity",
            "dnd5e2014.mount-vehicle-rule.other-mount-availability",
            "dnd5e2014.mount-vehicle-rule.barding",
            "dnd5e2014.mount-vehicle-rule.military-saddle",
            "dnd5e2014.mount-vehicle-rule.exotic-saddle",
            "dnd5e2014.mount-vehicle-rule.vehicle-proficiency",
            "dnd5e2014.mount-vehicle-rule.rowed-vessels"
        ];

        foreach (string value in ruleIds)
        {
            RuleDefinition definition =
                ruleset.Rules.Get(new RuleId(value));
            var source = Assert.Single(definition.Sources);

            Assert.Equal(155, source.Page);
            Assert.StartsWith(
                "Chapter 5: Equipment — Mounts and Vehicles",
                source.Section);
        }
    }

    [Fact]
    public void EveryTradeGoodSpecialRuleId_Resolves()
    {
        Dnd5e2014Ruleset ruleset = Dnd5e2014Ruleset.Instance;

        foreach (var tradeGood in ruleset.TradeGoods.All)
        {
            foreach (RuleId ruleId in tradeGood.SpecialRuleIds)
            {
                Assert.True(
                    ruleset.Rules.TryGet(
                        ruleId,
                        out RuleDefinition? definition));
                Assert.NotNull(definition);
            }
        }
    }

    [Fact]
    public void EveryRaceAndSubraceTraitRuleId_Resolves()
    {
        Dnd5e2014Ruleset ruleset = Dnd5e2014Ruleset.Instance;

        IEnumerable<RuleId> ids =
            ruleset.Races.All.SelectMany(race => race.TraitRuleIds)
                .Concat(
                    ruleset.Subraces.All.SelectMany(
                        subrace => subrace.TraitRuleIds));

        foreach (RuleId ruleId in ids)
        {
            Assert.True(
                ruleset.Rules.TryGet(
                    ruleId,
                    out RuleDefinition? definition));
            Assert.NotNull(definition);
        }
    }

    [Fact]
    public void EveryClassAndSubclassLevelFeatureRuleId_Resolves()
    {
        Dnd5e2014Ruleset ruleset = Dnd5e2014Ruleset.Instance;

        IEnumerable<RuleId> ids =
            ruleset.Classes.All
                .SelectMany(
                    @class => @class.LevelFeatures,
                    (_, feature) => feature.FeatureRuleId)
                .Concat(
                    ruleset.Subclasses.All.SelectMany(
                        subclass => subclass.LevelFeatures,
                        (_, feature) => feature.FeatureRuleId));

        foreach (RuleId ruleId in ids)
        {
            Assert.True(
                ruleset.Rules.TryGet(
                    ruleId,
                    out RuleDefinition? definition));
            Assert.NotNull(definition);
        }
    }

    [Fact]
    public void TradeGoodRule_PreservesFirstPrintingProvenance()
    {
        Dnd5e2014Ruleset ruleset = Dnd5e2014Ruleset.Instance;

        RuleDefinition definition = ruleset.Rules.Get(
            new RuleId(
                "dnd5e2014.trade-good-rule.full-value-and-currency"));

        Assert.Equal(
            new int?[] { 144, 157 },
            definition.Sources.Select(source => source.Page).ToArray());
        Assert.All(
            definition.Sources,
            source => Assert.StartsWith(
                "Chapter 5: Equipment",
                source.Section));
    }
}
