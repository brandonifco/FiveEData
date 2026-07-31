using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment.Armor;

namespace FiveEData.Tests;

public sealed class ArmorDefinitionValidatorTests
{
    [Theory]
    [InlineData(ArmorCategory.Light, true, null)]
    [InlineData(ArmorCategory.Medium, true, 2)]
    [InlineData(ArmorCategory.Heavy, false, null)]
    public void ValidCategorySemantics_AreAccepted(
        ArmorCategory category,
        bool includesDexterityModifier,
        int? maximumDexterityModifier)
    {
        ArmorDefinition armor = CreateArmor(
            category,
            includesDexterityModifier,
            maximumDexterityModifier,
            minimumStrengthForFullSpeed:
                category == ArmorCategory.Heavy ? 15 : null);

        Assert.Empty(ArmorDefinitionValidator.Validate(armor));
    }

    [Fact]
    public void LightArmor_WithoutDexterityModifier_IsRejected()
    {
        ArmorDefinition armor = CreateArmor(
            ArmorCategory.Light,
            includesDexterityModifier: false,
            maximumDexterityModifier: null);

        Assert.Contains(
            ArmorDefinitionValidator.Validate(armor),
            error => error.Contains("light armor", StringComparison.Ordinal));
    }

    [Fact]
    public void LightArmor_WithDexterityCap_IsRejected()
    {
        ArmorDefinition armor = CreateArmor(
            ArmorCategory.Light,
            includesDexterityModifier: true,
            maximumDexterityModifier: 2);

        Assert.Contains(
            ArmorDefinitionValidator.Validate(armor),
            error => error.Contains("full Dexterity", StringComparison.Ordinal));
    }

    [Fact]
    public void MediumArmor_WithoutPlusTwoCap_IsRejected()
    {
        ArmorDefinition armor = CreateArmor(
            ArmorCategory.Medium,
            includesDexterityModifier: true,
            maximumDexterityModifier: null);

        Assert.Contains(
            ArmorDefinitionValidator.Validate(armor),
            error => error.Contains("maximum of +2", StringComparison.Ordinal));
    }

    [Fact]
    public void HeavyArmor_WithDexterityModifier_IsRejected()
    {
        ArmorDefinition armor = CreateArmor(
            ArmorCategory.Heavy,
            includesDexterityModifier: true,
            maximumDexterityModifier: null);

        Assert.Contains(
            ArmorDefinitionValidator.Validate(armor),
            error => error.Contains("heavy armor", StringComparison.Ordinal));
    }

    [Fact]
    public void NonHeavyArmor_WithStrengthThreshold_IsRejected()
    {
        ArmorDefinition armor = CreateArmor(
            ArmorCategory.Medium,
            includesDexterityModifier: true,
            maximumDexterityModifier: 2,
            minimumStrengthForFullSpeed: 13);

        Assert.Contains(
            ArmorDefinitionValidator.Validate(armor),
            error => error.Contains("Strength threshold", StringComparison.Ordinal));
    }

    [Fact]
    public void ZeroCostWeightStrengthOrMissingSource_AreRejected()
    {
        ArmorDefinition armor = new(
            new ArmorId("dnd5e2014.armor.invalid"),
            "Invalid",
            ArmorCategory.Heavy,
            new Money(0),
            new Weight(0m),
            new ArmorClassFormula(16, includesDexterityModifier: false),
            minimumStrengthForFullSpeed: 0,
            imposesStealthDisadvantage: true,
            sources: []);

        IReadOnlyList<string> errors =
            ArmorDefinitionValidator.Validate(armor);

        Assert.Contains(errors, error => error.Contains("cost", StringComparison.Ordinal));
        Assert.Contains(errors, error => error.Contains("weight", StringComparison.Ordinal));
        Assert.Contains(errors, error => error.Contains("Strength", StringComparison.Ordinal));
        Assert.Contains(errors, error => error.Contains("source", StringComparison.Ordinal));
    }

    [Fact]
    public void DefaultArmorId_IsRejected()
    {
        ArmorDefinition armor = new(
            default,
            "Invalid",
            ArmorCategory.Heavy,
            new Money(1000),
            new Weight(10m),
            new ArmorClassFormula(16, includesDexterityModifier: false),
            minimumStrengthForFullSpeed: 15,
            imposesStealthDisadvantage: false,
            sources:
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 145)
            ]);

        Assert.Contains(
            ArmorDefinitionValidator.Validate(armor),
            error => error.Contains("ID", StringComparison.Ordinal));
    }

    [Fact]
    public void DefaultArmorClassFormula_IsRejected()
    {
        ArmorDefinition armor = new(
            new ArmorId("dnd5e2014.armor.invalid"),
            "Invalid",
            ArmorCategory.Heavy,
            new Money(1000),
            new Weight(10m),
            default,
            minimumStrengthForFullSpeed: 15,
            imposesStealthDisadvantage: false,
            sources:
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 145)
            ]);

        Assert.Contains(
            ArmorDefinitionValidator.Validate(armor),
            error => error.Contains("Base Armor Class", StringComparison.Ordinal));
    }

    private static ArmorDefinition CreateArmor(
        ArmorCategory category,
        bool includesDexterityModifier,
        int? maximumDexterityModifier,
        int? minimumStrengthForFullSpeed = null)
    {
        return new ArmorDefinition(
            new ArmorId("dnd5e2014.armor.test"),
            "Test armor",
            category,
            new Money(1000),
            new Weight(10m),
            new ArmorClassFormula(
                14,
                includesDexterityModifier,
                maximumDexterityModifier),
            minimumStrengthForFullSpeed,
            imposesStealthDisadvantage: false,
            sources:
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 145)
            ]);
    }
}
