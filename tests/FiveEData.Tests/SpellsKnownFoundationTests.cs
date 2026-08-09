using FiveEData.Rules.Classes.SpellsKnown;

namespace FiveEData.Tests;

public sealed class SpellsKnownFoundationTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(21)]
    public void Grant_RejectsOutOfRangeCharacterLevel(int characterLevel)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new SpellsKnownGrant(characterLevel, 2));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Grant_RejectsNonPositiveSpellsKnown(int spellsKnown)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new SpellsKnownGrant(1, spellsKnown));
    }

    [Fact]
    public void Detail_DefensivelySnapshotsSpellsKnownByLevel()
    {
        var spellsKnownByLevel = new List<SpellsKnownGrant> { new(1, 2) };

        var detail = new SpellsKnownProgressionDetail(spellsKnownByLevel);

        spellsKnownByLevel.Clear();

        Assert.Single(detail.SpellsKnownByLevel);
    }
}
