using FiveEData.Rules.Classes.BardicInspiration;
using FiveEData.Rules.Common;

namespace FiveEData.Tests;

public sealed class BardicInspirationFoundationTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(21)]
    public void DieGrant_RejectsOutOfRangeCharacterLevel(int characterLevel)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new BardicInspirationDieGrant(
                characterLevel,
                new DiceExpression(1, 6)));
    }

    [Fact]
    public void Detail_RejectsNonPositiveRangeFeet()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new BardicInspirationProgressionDetail(
                [new BardicInspirationDieGrant(1, new DiceExpression(1, 6))],
                rangeFeet: 0,
                durationMinutes: 10));
    }

    [Fact]
    public void Detail_RejectsNonPositiveDurationMinutes()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new BardicInspirationProgressionDetail(
                [new BardicInspirationDieGrant(1, new DiceExpression(1, 6))],
                rangeFeet: 60,
                durationMinutes: 0));
    }

    [Fact]
    public void Detail_DefensivelySnapshotsDieByLevel()
    {
        var dieByLevel = new List<BardicInspirationDieGrant>
        {
            new(1, new DiceExpression(1, 6))
        };

        var detail = new BardicInspirationProgressionDetail(
            dieByLevel,
            rangeFeet: 60,
            durationMinutes: 10);

        dieByLevel.Clear();

        Assert.Single(detail.DieByLevel);
    }

    [Fact]
    public void Detail_ExposesValues()
    {
        var detail = new BardicInspirationProgressionDetail(
            [new BardicInspirationDieGrant(1, new DiceExpression(1, 6))],
            rangeFeet: 60,
            durationMinutes: 10);

        Assert.Equal(60, detail.RangeFeet);
        Assert.Equal(10, detail.DurationMinutes);
    }
}
