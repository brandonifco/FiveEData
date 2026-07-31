using FiveEData.Rules.Equipment.Armor;

namespace FiveEData.Tests;

public sealed class ArmorClassFormulaTests
{
    [Fact]
    public void Formula_RejectsNonPositiveBaseArmorClass()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new ArmorClassFormula(
                0,
                includesDexterityModifier: true));
    }

    [Fact]
    public void Formula_RejectsDexterityMaximumWhenDexterityIsNotIncluded()
    {
        Assert.Throws<ArgumentException>(
            () => new ArmorClassFormula(
                14,
                includesDexterityModifier: false,
                maximumDexterityModifier: 2));
    }

    [Fact]
    public void Formula_RejectsNegativeDexterityMaximum()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new ArmorClassFormula(
                14,
                includesDexterityModifier: true,
                maximumDexterityModifier: -1));
    }
}
