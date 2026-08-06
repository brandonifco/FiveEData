using FiveEData.Rules.Classes.Auras;

namespace FiveEData.Tests;

public sealed class AurasFoundationTests
{
    [Fact]
    public void AuraRange_RejectsNonPositiveBaseRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new AuraRange(0, 30, 18));
    }

    [Fact]
    public void AuraRange_RejectsExpandedRangeNotGreaterThanBaseRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new AuraRange(10, 10, 18));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(21)]
    public void AuraRange_RejectsOutOfRangeExpandedAtLevel(int level)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new AuraRange(10, 30, level));
    }

    [Fact]
    public void AuraRange_ExposesValues()
    {
        var range = new AuraRange(10, 30, 18);

        Assert.Equal(10, range.BaseRangeFeet);
        Assert.Equal(30, range.ExpandedRangeFeet);
        Assert.Equal(18, range.ExpandedAtLevel);
    }

    [Fact]
    public void AuraOfProtectionDetail_RejectsNonPositiveSavingThrowBonusMinimum()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new AuraOfProtectionDetail(
                new AuraRange(10, 30, 18),
                requiresConsciousness: true,
                savingThrowBonusMinimum: 0));
    }

    [Fact]
    public void AuraOfProtectionDetail_ExposesValues()
    {
        var detail = new AuraOfProtectionDetail(
            new AuraRange(10, 30, 18),
            requiresConsciousness: true,
            savingThrowBonusMinimum: 1);

        Assert.Equal(10, detail.Range.BaseRangeFeet);
        Assert.True(detail.RequiresConsciousness);
        Assert.Equal(1, detail.SavingThrowBonusMinimum);
    }

    [Fact]
    public void AuraOfCourageDetail_ExposesValues()
    {
        var detail = new AuraOfCourageDetail(
            new AuraRange(10, 30, 18),
            requiresConsciousness: true);

        Assert.Equal(10, detail.Range.BaseRangeFeet);
        Assert.True(detail.RequiresConsciousness);
    }

    [Fact]
    public void AuraOfDevotionDetail_ExposesValues()
    {
        var detail = new AuraOfDevotionDetail(
            new AuraRange(10, 30, 18),
            requiresConsciousness: true);

        Assert.Equal(10, detail.Range.BaseRangeFeet);
        Assert.True(detail.RequiresConsciousness);
    }

    [Fact]
    public void AuraOfWardingDetail_ExposesValuesWithoutRequiringConsciousness()
    {
        var detail = new AuraOfWardingDetail(
            new AuraRange(10, 30, 18),
            requiresConsciousness: false);

        Assert.Equal(10, detail.Range.BaseRangeFeet);
        Assert.False(detail.RequiresConsciousness);
    }
}
