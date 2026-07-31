using FiveEData.Rules.Common;

namespace FiveEData.Rules.Equipment.Weapons;

public sealed record WeaponDamage
{
    public WeaponDamage(
        DiceExpression? dice,
        int fixedAmount,
        DamageType type)
    {
        if (fixedAmount < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(fixedAmount),
                fixedAmount,
                "Fixed damage cannot be negative.");
        }

        if (dice is null && fixedAmount == 0)
        {
            throw new ArgumentException(
                "Weapon damage must define either damage dice or a positive fixed amount.");
        }

        if (dice is not null && fixedAmount != 0)
        {
            throw new ArgumentException(
                "Weapon damage cannot define both damage dice and fixed damage.");
        }

        Dice = dice;
        FixedAmount = fixedAmount;
        Type = type;
    }

    public DiceExpression? Dice { get; }

    public int FixedAmount { get; }

    public DamageType Type { get; }
}
