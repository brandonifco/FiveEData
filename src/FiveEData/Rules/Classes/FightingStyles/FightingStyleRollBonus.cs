namespace FiveEData.Rules.Classes.FightingStyles;

public readonly record struct FightingStyleRollBonus
{
    public FightingStyleRollBonus(
        FightingStyleRollTarget target,
        int amount,
        FightingStyleWeaponRequirement weaponRequirement)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                amount,
                "Fighting style roll bonus amount must be greater than zero.");
        }

        Target = target;
        Amount = amount;
        WeaponRequirement = weaponRequirement;
    }

    public FightingStyleRollTarget Target { get; }

    public int Amount { get; }

    public FightingStyleWeaponRequirement WeaponRequirement { get; }
}
