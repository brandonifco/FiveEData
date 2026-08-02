using FiveEData.Rules.Common;
using FiveEData.Rules.Expenses.Services;

namespace FiveEData.Tests;

public sealed class MundaneServiceRulesetIntegrationTests
{
    [Fact]
    public void EmbeddedRuleset_ExposesCanonicalMundaneServices()
    {
        Dnd5e2014Ruleset ruleset =
            Dnd5e2014Ruleset.Instance;

        Assert.Equal(
            7,
            ruleset.Expenses.MundaneServices.Count);

        MundaneServiceDefinition skilled =
            ruleset.Expenses.MundaneServices.Get(
                new MundaneServiceId(
                    "dnd5e2014.mundane-service." +
                    "hireling-skilled"));

        Assert.Equal("Hireling, skilled", skilled.Name);
        Assert.Equal(
            200,
            skilled.Cost.Amount.CopperPieces);
        Assert.Equal(
            ListedCostKind.Minimum,
            skilled.Cost.Kind);
        Assert.Equal(
            ServicePricingUnit.Day,
            skilled.PricingUnit);
        Assert.Equal(2, skilled.SpecialRuleIds.Count);

        Assert.DoesNotContain(
            ruleset.Expenses.MundaneServices.All,
            definition =>
                definition.Name.Contains(
                    "spellcasting",
                    StringComparison.OrdinalIgnoreCase));
    }
}
