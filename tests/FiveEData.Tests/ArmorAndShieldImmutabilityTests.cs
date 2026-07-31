using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment.Armor;
using FiveEData.Rules.Equipment.Shields;

namespace FiveEData.Tests;

public sealed class ArmorAndShieldImmutabilityTests
{
    [Fact]
    public void ArmorDefinition_DefensivelySnapshotsSources()
    {
        var sources = new List<SourceReference>
        {
            new(
                new SourceDocumentId(
                    "dnd5e2014.source.phb-first-printing"),
                page: 145)
        };

        ArmorDefinition armor = new(
            new ArmorId("dnd5e2014.armor.test"),
            "Test armor",
            ArmorCategory.Light,
            new Money(1000),
            new Weight(10m),
            new ArmorClassFormula(11, includesDexterityModifier: true),
            minimumStrengthForFullSpeed: null,
            imposesStealthDisadvantage: false,
            sources);

        sources.Add(
            new SourceReference(
                new SourceDocumentId("dnd5e2014.source.other"),
                page: 1));

        Assert.Single(armor.Sources);
        Assert.Equal(
            new SourceDocumentId(
                "dnd5e2014.source.phb-first-printing"),
            armor.Sources[0].DocumentId);
    }

    [Fact]
    public void ShieldDefinition_DefensivelySnapshotsSources()
    {
        var sources = new List<SourceReference>
        {
            new(
                new SourceDocumentId(
                    "dnd5e2014.source.phb-first-printing"),
                page: 145)
        };

        ShieldDefinition shield = new(
            new ShieldId("dnd5e2014.armor.shield-test"),
            "Test shield",
            new Money(1000),
            new Weight(6m),
            armorClassBonus: 2,
            sources);

        sources.Add(
            new SourceReference(
                new SourceDocumentId("dnd5e2014.source.other"),
                page: 1));

        Assert.Single(shield.Sources);
        Assert.Equal(
            new SourceDocumentId(
                "dnd5e2014.source.phb-first-printing"),
            shield.Sources[0].DocumentId);
    }

    [Theory]
    [InlineData(typeof(ArmorDefinition))]
    [InlineData(typeof(ShieldDefinition))]
    public void Definitions_ExposeNoPublicSetters(Type type)
    {
        Assert.All(
            type.GetProperties(),
            property => Assert.Null(property.SetMethod));
    }

    [Fact]
    public void IdentityBearingDefinitions_UseReferenceIdentity()
    {
        ArmorDefinition firstArmor = CreateArmor();
        ArmorDefinition secondArmor = CreateArmor();
        ShieldDefinition firstShield = CreateShield();
        ShieldDefinition secondShield = CreateShield();

        Assert.NotSame(firstArmor, secondArmor);
        Assert.False(firstArmor.Equals(secondArmor));
        Assert.NotSame(firstShield, secondShield);
        Assert.False(firstShield.Equals(secondShield));
    }

    private static ArmorDefinition CreateArmor()
    {
        return new ArmorDefinition(
            new ArmorId("dnd5e2014.armor.identity-test"),
            "Identity Test",
            ArmorCategory.Light,
            new Money(1000),
            new Weight(10m),
            new ArmorClassFormula(11, includesDexterityModifier: true),
            minimumStrengthForFullSpeed: null,
            imposesStealthDisadvantage: false,
            sources:
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 145)
            ]);
    }

    private static ShieldDefinition CreateShield()
    {
        return new ShieldDefinition(
            new ShieldId("dnd5e2014.armor.shield-identity-test"),
            "Identity Test",
            new Money(1000),
            new Weight(6m),
            armorClassBonus: 2,
            sources:
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 145)
            ]);
    }
}
