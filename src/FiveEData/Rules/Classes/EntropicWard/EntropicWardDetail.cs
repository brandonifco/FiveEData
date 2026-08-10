using FiveEData.Rules.Common;

namespace FiveEData.Rules.Classes.EntropicWard;

public sealed record EntropicWardDetail
{
    public EntropicWardDetail(
        bool imposesDisadvantageOnTriggeringAttackRoll,
        bool grantsAdvantageOnNextAttackRollIfMissed,
        NextTurnDurationTrigger advantageDurationTrigger,
        bool recoversOnShortRest)
    {
        if (!Enum.IsDefined(advantageDurationTrigger))
        {
            throw new ArgumentOutOfRangeException(
                nameof(advantageDurationTrigger),
                advantageDurationTrigger,
                "Entropic Ward advantage duration trigger must be defined.");
        }

        ImposesDisadvantageOnTriggeringAttackRoll =
            imposesDisadvantageOnTriggeringAttackRoll;
        GrantsAdvantageOnNextAttackRollIfMissed =
            grantsAdvantageOnNextAttackRollIfMissed;
        AdvantageDurationTrigger = advantageDurationTrigger;
        RecoversOnShortRest = recoversOnShortRest;
    }

    public bool ImposesDisadvantageOnTriggeringAttackRoll { get; }

    public bool GrantsAdvantageOnNextAttackRollIfMissed { get; }

    public NextTurnDurationTrigger AdvantageDurationTrigger { get; }

    public bool RecoversOnShortRest { get; }
}
