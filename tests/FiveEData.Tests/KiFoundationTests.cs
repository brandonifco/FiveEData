using FiveEData.Rules.Classes.Ki;

namespace FiveEData.Tests;

public sealed class KiFoundationTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(21)]
    public void PointsGrant_RejectsOutOfRangeCharacterLevel(int characterLevel)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new KiPointsGrant(characterLevel, 2));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void PointsGrant_RejectsNonPositivePoints(int points)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new KiPointsGrant(2, points));
    }

    [Fact]
    public void Detail_DefensivelySnapshotsPointsByLevel()
    {
        var pointsByLevel = new List<KiPointsGrant> { new(2, 2) };

        var detail = new KiProgressionDetail(
            pointsByLevel,
            recoversOnShortRest: true);

        pointsByLevel.Clear();

        Assert.Single(detail.PointsByLevel);
    }

    [Fact]
    public void Detail_ExposesRecoversOnShortRest()
    {
        var detail = new KiProgressionDetail(
            [new KiPointsGrant(2, 2)],
            recoversOnShortRest: true);

        Assert.True(detail.RecoversOnShortRest);
    }
}
