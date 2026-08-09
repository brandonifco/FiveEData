using FiveEData.Rules.Classes.CantripsKnown;

namespace FiveEData.Tests;

public sealed class CantripsKnownFoundationTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(21)]
    public void Grant_RejectsOutOfRangeCharacterLevel(int characterLevel)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new CantripsKnownGrant(characterLevel, 2));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Grant_RejectsNonPositiveCantripsKnown(int cantripsKnown)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new CantripsKnownGrant(1, cantripsKnown));
    }

    [Fact]
    public void Detail_DefensivelySnapshotsCantripsKnownByLevel()
    {
        var cantripsKnownByLevel = new List<CantripsKnownGrant> { new(1, 2) };

        var detail =
            new CantripsKnownProgressionDetail(cantripsKnownByLevel);

        cantripsKnownByLevel.Clear();

        Assert.Single(detail.CantripsKnownByLevel);
    }
}
