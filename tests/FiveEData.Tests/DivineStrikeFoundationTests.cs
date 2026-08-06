using FiveEData.Rules.Classes.DivineStrike;
using FiveEData.Rules.Common;
using FiveEData.Rules.Creatures.DamageTypes;

namespace FiveEData.Tests;

public sealed class DivineStrikeFoundationTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(21)]
    public void DamageGrant_RejectsOutOfRangeCharacterLevel(
        int characterLevel)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new DivineStrikeDamageGrant(
                characterLevel,
                new DiceExpression(1, 8)));
    }

    [Fact]
    public void Detail_DefensivelySnapshotsDamageByLevelAndChoosableDamageTypeIds()
    {
        var damageByLevel = new List<DivineStrikeDamageGrant>
        {
            new(8, new DiceExpression(1, 8))
        };
        var choosableDamageTypeIds = new List<DamageTypeId>
        {
            new("dnd5e2014.damage-type.cold")
        };

        var detail = new DivineStrikeProgressionDetail(
            damageByLevel,
            fixedDamageTypeId: null,
            choosableDamageTypeIds,
            matchesWeaponDamageType: false);

        damageByLevel.Clear();
        choosableDamageTypeIds.Clear();

        Assert.Single(detail.DamageByLevel);
        Assert.Single(detail.ChoosableDamageTypeIds!);
    }

    [Fact]
    public void Detail_ExposesFixedDamageType()
    {
        var detail = new DivineStrikeProgressionDetail(
            [new DivineStrikeDamageGrant(8, new DiceExpression(1, 8))],
            fixedDamageTypeId: new DamageTypeId(
                "dnd5e2014.damage-type.radiant"),
            choosableDamageTypeIds: null,
            matchesWeaponDamageType: false);

        Assert.Equal(
            "dnd5e2014.damage-type.radiant",
            detail.FixedDamageTypeId?.Value);
        Assert.Null(detail.ChoosableDamageTypeIds);
        Assert.False(detail.MatchesWeaponDamageType);
    }

    [Fact]
    public void Detail_ExposesMatchesWeaponDamageType()
    {
        var detail = new DivineStrikeProgressionDetail(
            [new DivineStrikeDamageGrant(8, new DiceExpression(1, 8))],
            fixedDamageTypeId: null,
            choosableDamageTypeIds: null,
            matchesWeaponDamageType: true);

        Assert.Null(detail.FixedDamageTypeId);
        Assert.Null(detail.ChoosableDamageTypeIds);
        Assert.True(detail.MatchesWeaponDamageType);
    }
}
