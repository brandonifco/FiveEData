using FiveEData.Rules.Classes.WildShape;

namespace FiveEData.Tests;

public sealed class WildShapeFoundationTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(21)]
    public void FormLimit_RejectsOutOfRangeCharacterLevel(int characterLevel)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new WildShapeFormLimit(characterLevel, 0.25, false, false));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void FormLimit_RejectsNonPositiveMaxChallengeRating(
        double maxChallengeRating)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new WildShapeFormLimit(2, maxChallengeRating, false, false));
    }

    [Fact]
    public void Detail_RejectsNonPositiveUsesPerRest()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new WildShapeProgressionDetail(
                [new WildShapeFormLimit(2, 0.25, false, false)],
                usesPerRest: 0,
                recoversOnShortRest: true));
    }

    [Fact]
    public void Detail_DefensivelySnapshotsFormLimitsByLevel()
    {
        var formLimits = new List<WildShapeFormLimit>
        {
            new(2, 0.25, false, false)
        };

        var detail = new WildShapeProgressionDetail(
            formLimits,
            usesPerRest: 2,
            recoversOnShortRest: true);

        formLimits.Clear();

        Assert.Single(detail.FormLimitsByLevel);
    }

    [Fact]
    public void Detail_ExposesUsesAndRecovery()
    {
        var detail = new WildShapeProgressionDetail(
            [new WildShapeFormLimit(2, 0.25, false, false)],
            usesPerRest: 2,
            recoversOnShortRest: true);

        Assert.Equal(2, detail.UsesPerRest);
        Assert.True(detail.RecoversOnShortRest);
    }
}
