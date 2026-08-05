using FiveEData.Rules.Common;
using FiveEData.Rules.Creatures.DamageTypes;

namespace FiveEData.Rules.Equipment.Weapons;

public sealed record WeaponDamage
{
    public WeaponDamage(
        DiceExpression? dice,
        int fixedAmount,
        DamageTypeId damageTypeId)
    {
        if (string.IsNullOrWhiteSpace(damageTypeId.Value))
        {
            throw new ArgumentException(
                "Weapon damage type ID must not be empty.",
                nameof(damageTypeId));
        }

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
        DamageTypeId = damageTypeId;
    }

    public DiceExpression? Dice { get; }

    public int FixedAmount { get; }

    public DamageTypeId DamageTypeId { get; }
}
