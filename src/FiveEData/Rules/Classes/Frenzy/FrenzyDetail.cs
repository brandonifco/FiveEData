namespace FiveEData.Rules.Classes.Frenzy;

public sealed record FrenzyDetail
{
    public FrenzyDetail(
        bool grantsBonusActionMeleeAttack,
        int exhaustionLevelsWhenRageEnds)
    {
        if (exhaustionLevelsWhenRageEnds <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(exhaustionLevelsWhenRageEnds),
                exhaustionLevelsWhenRageEnds,
                "Frenzy exhaustion levels when rage ends must be greater " +
                "than zero.");
        }

        GrantsBonusActionMeleeAttack = grantsBonusActionMeleeAttack;
        ExhaustionLevelsWhenRageEnds = exhaustionLevelsWhenRageEnds;
    }

    public bool GrantsBonusActionMeleeAttack { get; }

    public int ExhaustionLevelsWhenRageEnds { get; }
}
