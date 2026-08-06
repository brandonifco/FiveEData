using FiveEData.Rules.Classes.SongOfRest;
using FiveEData.Rules.Common;

namespace FiveEData.Tests;

public sealed class SongOfRestFoundationTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(21)]
    public void DieGrant_RejectsOutOfRangeCharacterLevel(int characterLevel)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new SongOfRestDieGrant(
                characterLevel,
                new DiceExpression(1, 6)));
    }

    [Fact]
    public void Detail_DefensivelySnapshotsDieByLevel()
    {
        var dieByLevel = new List<SongOfRestDieGrant>
        {
            new(2, new DiceExpression(1, 6))
        };

        var detail = new SongOfRestProgressionDetail(dieByLevel);

        dieByLevel.Clear();

        Assert.Single(detail.DieByLevel);
    }
}
