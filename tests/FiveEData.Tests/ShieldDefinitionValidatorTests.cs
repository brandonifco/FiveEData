using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment.Shields;

namespace FiveEData.Tests;

public sealed class ShieldDefinitionValidatorTests
{
    [Fact]
    public void ValidShield_IsAccepted()
    {
        ShieldDefinition shield = CreateShield(
            new Money(1000),
            new Weight(6m),
            armorClassBonus: 2,
            withSource: true);

        Assert.Empty(ShieldDefinitionValidator.Validate(shield));
    }

    [Fact]
    public void InvalidShieldFacts_AreRejected()
    {
        ShieldDefinition shield = CreateShield(
            new Money(0),
            new Weight(0m),
            armorClassBonus: 0,
            withSource: false);

        IReadOnlyList<string> errors =
            ShieldDefinitionValidator.Validate(shield);

        Assert.Contains(errors, error => error.Contains("cost", StringComparison.Ordinal));
        Assert.Contains(errors, error => error.Contains("weight", StringComparison.Ordinal));
        Assert.Contains(errors, error => error.Contains("Armor Class", StringComparison.Ordinal));
        Assert.Contains(errors, error => error.Contains("source", StringComparison.Ordinal));
    }

    private static ShieldDefinition CreateShield(
        Money cost,
        Weight weight,
        int armorClassBonus,
        bool withSource)
    {
        SourceReference[] sources = withSource
            ?
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 145)
            ]
            : [];

        return new ShieldDefinition(
            new ShieldId("dnd5e2014.armor.shield-test"),
            "Test shield",
            cost,
            weight,
            armorClassBonus,
            sources);
    }
}
