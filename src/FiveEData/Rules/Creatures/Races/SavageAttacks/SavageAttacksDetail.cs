namespace FiveEData.Rules.Creatures.Races.SavageAttacks;

public sealed record SavageAttacksDetail
{
    public SavageAttacksDetail(
        int additionalCriticalDice,
        bool requiresMeleeWeapon)
    {
        if (additionalCriticalDice <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(additionalCriticalDice),
                additionalCriticalDice,
                "Savage Attacks additional critical dice must be greater " +
                "than zero.");
        }

        AdditionalCriticalDice = additionalCriticalDice;
        RequiresMeleeWeapon = requiresMeleeWeapon;
    }

    public int AdditionalCriticalDice { get; }

    public bool RequiresMeleeWeapon { get; }
}
