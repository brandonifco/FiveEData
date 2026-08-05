using FiveEData.Rules.Common;
using FiveEData.Rules.Creatures.DamageTypes;
using FiveEData.Rules.Equipment.Weapons;

namespace FiveEData.Tests;

public sealed class WeaponDamageTests
{
    private static readonly DamageTypeId Bludgeoning =
        new("dnd5e2014.damage-type.bludgeoning");
    private static readonly DamageTypeId Piercing =
        new("dnd5e2014.damage-type.piercing");
    private static readonly DamageTypeId Slashing =
        new("dnd5e2014.damage-type.slashing");

    [Fact]
    public void DiceDamage_WithDefinedType_IsAccepted()
    {
        var dice = new DiceExpression(1, 8);

        var damage = new WeaponDamage(
            dice,
            fixedAmount: 0,
            Slashing);

        Assert.Equal(dice, damage.Dice);
        Assert.Equal(0, damage.FixedAmount);
        Assert.Equal(Slashing, damage.DamageTypeId);
    }

    [Fact]
    public void FixedDamage_WithDefinedType_IsAccepted()
    {
        var damage = new WeaponDamage(
            dice: null,
            fixedAmount: 1,
            Piercing);

        Assert.Null(damage.Dice);
        Assert.Equal(1, damage.FixedAmount);
        Assert.Equal(Piercing, damage.DamageTypeId);
    }

    [Fact]
    public void DefaultDamageTypeId_IsRejected()
    {
        ArgumentException exception =
            Assert.Throws<ArgumentException>(
                () => new WeaponDamage(
                    new DiceExpression(1, 8),
                    fixedAmount: 0,
                    default(DamageTypeId)));

        Assert.Equal("damageTypeId", exception.ParamName);
    }

    [Fact]
    public void MissingDiceAndFixedDamage_RemainsRejected()
    {
        Assert.Throws<ArgumentException>(
            () => new WeaponDamage(
                dice: null,
                fixedAmount: 0,
                Bludgeoning));
    }

    [Fact]
    public void DiceAndFixedDamageTogether_RemainRejected()
    {
        Assert.Throws<ArgumentException>(
            () => new WeaponDamage(
                new DiceExpression(1, 4),
                fixedAmount: 1,
                Bludgeoning));
    }

    [Fact]
    public void NegativeFixedDamage_RemainsRejected()
    {
        ArgumentOutOfRangeException exception =
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new WeaponDamage(
                    dice: null,
                    fixedAmount: -1,
                    Bludgeoning));

        Assert.Equal("fixedAmount", exception.ParamName);
    }
}
