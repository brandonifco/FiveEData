using FiveEData.Rules.Expenses.Lifestyles;

namespace FiveEData.Tests;

public sealed class
    LifestyleHospitalityCostRulesetIntegrationTests
{
    [Fact]
    public void EmbeddedRuleset_ExposesCanonicalHospitalityCosts()
    {
        Dnd5e2014Ruleset ruleset =
            Dnd5e2014Ruleset.Instance;

        Assert.Equal(
            6,
            ruleset.Expenses.HospitalityCosts.Count);

        var modest =
            ruleset.Expenses.HospitalityCosts.Get(
                new LifestyleId(
                    "dnd5e2014.lifestyle.modest"));

        Assert.Equal(
            50,
            modest.InnStayCostPerDay.CopperPieces);
        Assert.Equal(
            30,
            modest.MealsCostPerDay.CopperPieces);

        Assert.False(
            ruleset.Expenses.HospitalityCosts.TryGet(
                new LifestyleId(
                    "dnd5e2014.lifestyle.wretched"),
                out _));
    }
}
