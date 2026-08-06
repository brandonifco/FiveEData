using FiveEData.Rules.Classes.FontOfMagic;

namespace FiveEData.Tests;

public sealed class FontOfMagicFoundationTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    public void SlotCostGrant_RejectsOutOfRangeSpellSlotLevel(
        int spellSlotLevel)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new FontOfMagicSlotCostGrant(spellSlotLevel, 2));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void SlotCostGrant_RejectsNonPositiveSorceryPointCost(
        int sorceryPointCost)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new FontOfMagicSlotCostGrant(1, sorceryPointCost));
    }

    [Fact]
    public void Detail_DefensivelySnapshotsSlotCostByLevel()
    {
        var slotCostByLevel = new List<FontOfMagicSlotCostGrant>
        {
            new(1, 2)
        };

        var detail = new FontOfMagicConversionDetail(slotCostByLevel);

        slotCostByLevel.Clear();

        Assert.Single(detail.SlotCostByLevel);
    }
}
