using FiveEData.Rules.Common;
using FiveEData.Rules.Equipment.Weapons;

namespace FiveEData.Tests;

public sealed class WeaponDamageTests
{
    [Fact]
    public void DiceDamage_WithDefinedType_IsAccepted()
    {
        var dice = new DiceExpression(1, 8);

        var damage = new WeaponDamage(
            dice,
            fixedAmount: 0,
            DamageType.Slashing);

        Assert.Equal(dice, damage.Dice);
        Assert.Equal(0, damage.FixedAmount);
        Assert.Equal(DamageType.Slashing, damage.Type);
    }

    [Fact]
    public void FixedDamage_WithDefinedType_IsAccepted()
    {
        var damage = new WeaponDamage(
            dice: null,
            fixedAmount: 1,
            DamageType.Piercing);

        Assert.Null(damage.Dice);
        Assert.Equal(1, damage.FixedAmount);
        Assert.Equal(DamageType.Piercing, damage.Type);
    }

    [Fact]
    public void UndefinedDamageType_IsRejected()
    {
        ArgumentOutOfRangeException exception =
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new WeaponDamage(
                    new DiceExpression(1, 8),
                    fixedAmount: 0,
                    (DamageType)999));

        Assert.Equal("type", exception.ParamName);
    }

    [Fact]
    public void MissingDiceAndFixedDamage_RemainsRejected()
    {
        Assert.Throws<ArgumentException>(
            () => new WeaponDamage(
                dice: null,
                fixedAmount: 0,
                DamageType.Bludgeoning));
    }

    [Fact]
    public void DiceAndFixedDamageTogether_RemainRejected()
    {
        Assert.Throws<ArgumentException>(
            () => new WeaponDamage(
                new DiceExpression(1, 4),
                fixedAmount: 1,
                DamageType.Bludgeoning));
    }

    [Fact]
    public void NegativeFixedDamage_RemainsRejected()
    {
        ArgumentOutOfRangeException exception =
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new WeaponDamage(
                    dice: null,
                    fixedAmount: -1,
                    DamageType.Bludgeoning));

        Assert.Equal("fixedAmount", exception.ParamName);
    }
}
