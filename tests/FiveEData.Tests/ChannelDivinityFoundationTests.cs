using FiveEData.Rules.Classes.ChannelDivinity;

namespace FiveEData.Tests;

public sealed class ChannelDivinityFoundationTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(21)]
    public void UseGrant_RejectsOutOfRangeCharacterLevel(int characterLevel)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new ChannelDivinityUseGrant(characterLevel, 1));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void UseGrant_RejectsNonPositiveUsesPerRest(int usesPerRest)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new ChannelDivinityUseGrant(2, usesPerRest));
    }

    [Fact]
    public void Detail_DefensivelySnapshotsUsesByLevel()
    {
        var usesByLevel = new List<ChannelDivinityUseGrant> { new(2, 1) };

        var detail = new ChannelDivinityProgressionDetail(
            usesByLevel,
            recoversOnShortRest: true);

        usesByLevel.Clear();

        Assert.Single(detail.UsesByLevel);
    }

    [Fact]
    public void Detail_ExposesRecoversOnShortRest()
    {
        var detail = new ChannelDivinityProgressionDetail(
            [new ChannelDivinityUseGrant(2, 1)],
            recoversOnShortRest: true);

        Assert.True(detail.RecoversOnShortRest);
    }
}
