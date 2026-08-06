using FiveEData.Rules.Classes.MysticArcanum;

namespace FiveEData.Tests;

public sealed class MysticArcanumFoundationTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(21)]
    public void Grant_RejectsOutOfRangeCharacterLevel(int characterLevel)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new MysticArcanumGrant(characterLevel, 6));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    public void Grant_RejectsOutOfRangeSpellLevel(int spellLevel)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new MysticArcanumGrant(11, spellLevel));
    }

    [Fact]
    public void Detail_DefensivelySnapshotsArcanumByLevel()
    {
        var arcanumByLevel = new List<MysticArcanumGrant> { new(11, 6) };

        var detail = new MysticArcanumProgressionDetail(
            arcanumByLevel,
            recoversOnShortRest: false);

        arcanumByLevel.Clear();

        Assert.Single(detail.ArcanumByLevel);
    }

    [Fact]
    public void Detail_ExposesRecoversOnShortRest()
    {
        var detail = new MysticArcanumProgressionDetail(
            [new MysticArcanumGrant(11, 6)],
            recoversOnShortRest: false);

        Assert.False(detail.RecoversOnShortRest);
    }
}
