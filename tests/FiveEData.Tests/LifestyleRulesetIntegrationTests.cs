using FiveEData.Rules.Common;
using FiveEData.Rules.Expenses.Lifestyles;

namespace FiveEData.Tests;

public sealed class LifestyleRulesetIntegrationTests
{
    [Fact]
    public void EmbeddedRuleset_ExposesCanonicalLifestyles()
    {
        Dnd5e2014Ruleset ruleset =
            Dnd5e2014Ruleset.Instance;

        Assert.Equal(7, ruleset.Expenses.Lifestyles.Count);

        LifestyleDefinition wretched =
            ruleset.Expenses.Lifestyles.Get(
                new LifestyleId(
                    "dnd5e2014.lifestyle.wretched"));

        Assert.Null(wretched.DailyCost);

        LifestyleDefinition aristocratic =
            ruleset.Expenses.Lifestyles.Get(
                new LifestyleId(
                    "dnd5e2014.lifestyle.aristocratic"));

        Assert.Equal(
            1000,
            aristocratic.DailyCost?.Amount.CopperPieces);
        Assert.Equal(
            ListedCostKind.Minimum,
            aristocratic.DailyCost?.Kind);
    }
}
