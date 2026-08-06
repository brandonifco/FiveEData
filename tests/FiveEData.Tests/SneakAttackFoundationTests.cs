using FiveEData.Rules.Classes.SneakAttack;
using FiveEData.Rules.Common;

namespace FiveEData.Tests;

public sealed class SneakAttackFoundationTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(21)]
    public void DiceGrant_RejectsOutOfRangeCharacterLevel(
        int characterLevel)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new SneakAttackDiceGrant(
                characterLevel,
                new DiceExpression(1, 6)));
    }

    [Fact]
    public void Detail_DefensivelySnapshotsDiceByLevel()
    {
        var dice = new List<SneakAttackDiceGrant>
        {
            new(1, new DiceExpression(1, 6))
        };

        var detail = new SneakAttackProgressionDetail(dice, true, true);

        dice.Clear();

        Assert.Single(detail.DiceByLevel);
    }

    [Fact]
    public void Detail_ExposesFlags()
    {
        var detail = new SneakAttackProgressionDetail(
            [new SneakAttackDiceGrant(1, new DiceExpression(1, 6))],
            oncePerTurn: true,
            requiresFinesseOrRangedWeapon: true);

        Assert.True(detail.OncePerTurn);
        Assert.True(detail.RequiresFinesseOrRangedWeapon);
    }
}
