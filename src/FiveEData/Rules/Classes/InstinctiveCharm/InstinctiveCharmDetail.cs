using FiveEData.Rules.Creatures.Abilities;

namespace FiveEData.Rules.Classes.InstinctiveCharm;

public sealed record InstinctiveCharmDetail
{
    public InstinctiveCharmDetail(
        int rangeFeet,
        AbilityId savingThrowAbilityId,
        bool redirectsAttackToClosestOtherCreature)
    {
        if (rangeFeet <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(rangeFeet),
                rangeFeet,
                "Instinctive Charm range must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(savingThrowAbilityId.Value))
        {
            throw new ArgumentException(
                "Instinctive Charm saving throw ability ID is required.",
                nameof(savingThrowAbilityId));
        }

        RangeFeet = rangeFeet;
        SavingThrowAbilityId = savingThrowAbilityId;
        RedirectsAttackToClosestOtherCreature =
            redirectsAttackToClosestOtherCreature;
    }

    public int RangeFeet { get; }

    public AbilityId SavingThrowAbilityId { get; }

    public bool RedirectsAttackToClosestOtherCreature { get; }
}
