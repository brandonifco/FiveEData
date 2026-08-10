using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Conditions;

namespace FiveEData.Rules.Classes.DarkDelirium;

public sealed record DarkDeliriumDetail
{
    public DarkDeliriumDetail(
        int rangeFeet,
        AbilityId savingThrowAbilityId,
        IEnumerable<ConditionId> choosableConditionIds,
        int durationMinutes,
        bool requiresConcentration,
        bool recoversOnShortRest)
    {
        if (rangeFeet <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(rangeFeet),
                rangeFeet,
                "Dark Delirium range must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(savingThrowAbilityId.Value))
        {
            throw new ArgumentException(
                "Dark Delirium saving throw ability ID is required.",
                nameof(savingThrowAbilityId));
        }

        ArgumentNullException.ThrowIfNull(choosableConditionIds);

        ConditionId[] choosableConditionIdArray =
            choosableConditionIds.ToArray();

        if (choosableConditionIdArray.Length < 2)
        {
            throw new ArgumentException(
                "Dark Delirium choosable condition IDs must contain at " +
                "least two options.",
                nameof(choosableConditionIds));
        }

        if (durationMinutes <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(durationMinutes),
                durationMinutes,
                "Dark Delirium duration must be greater than zero.");
        }

        RangeFeet = rangeFeet;
        SavingThrowAbilityId = savingThrowAbilityId;
        ChoosableConditionIds = Array.AsReadOnly(choosableConditionIdArray);
        DurationMinutes = durationMinutes;
        RequiresConcentration = requiresConcentration;
        RecoversOnShortRest = recoversOnShortRest;
    }

    public int RangeFeet { get; }

    public AbilityId SavingThrowAbilityId { get; }

    public IReadOnlyList<ConditionId> ChoosableConditionIds { get; }

    public int DurationMinutes { get; }

    public bool RequiresConcentration { get; }

    public bool RecoversOnShortRest { get; }
}
