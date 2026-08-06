namespace FiveEData.Rules.Classes.FightingStyles;

public readonly record struct FightingStyleDamageDieReroll
{
    public FightingStyleDamageDieReroll(
        int rerollAtOrBelowValue,
        FightingStyleWeaponRequirement weaponRequirement)
    {
        if (rerollAtOrBelowValue <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(rerollAtOrBelowValue),
                rerollAtOrBelowValue,
                "Fighting style damage die reroll threshold must be " +
                "greater than zero.");
        }

        RerollAtOrBelowValue = rerollAtOrBelowValue;
        WeaponRequirement = weaponRequirement;
    }

    public int RerollAtOrBelowValue { get; }

    public FightingStyleWeaponRequirement WeaponRequirement { get; }
}
