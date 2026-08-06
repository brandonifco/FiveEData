using FiveEData.Rules.Classes.SorceryPoints;

namespace FiveEData.Tests;

public sealed class SorceryPointsFoundationTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(21)]
    public void PointsGrant_RejectsOutOfRangeCharacterLevel(int characterLevel)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new SorceryPointsGrant(characterLevel, 2));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void PointsGrant_RejectsNonPositivePoints(int points)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new SorceryPointsGrant(2, points));
    }

    [Fact]
    public void Detail_DefensivelySnapshotsPointsByLevel()
    {
        var pointsByLevel = new List<SorceryPointsGrant> { new(2, 2) };

        var detail = new SorceryPointsProgressionDetail(
            pointsByLevel,
            recoversOnShortRest: false);

        pointsByLevel.Clear();

        Assert.Single(detail.PointsByLevel);
    }

    [Fact]
    public void Detail_ExposesRecoversOnShortRest()
    {
        var detail = new SorceryPointsProgressionDetail(
            [new SorceryPointsGrant(2, 2)],
            recoversOnShortRest: false);

        Assert.False(detail.RecoversOnShortRest);
    }
}
