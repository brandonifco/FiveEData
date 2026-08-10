using FiveEData.Rules.Common;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Conditions;

namespace FiveEData.Rules.Classes.FeyPresence;

public sealed record FeyPresenceDetail
{
    public FeyPresenceDetail(
        int areaSizeFeet,
        AbilityId savingThrowAbilityId,
        IEnumerable<ConditionId> choosableConditionIds,
        NextTurnDurationTrigger conditionDurationTrigger,
        bool recoversOnShortRest)
    {
        if (areaSizeFeet <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(areaSizeFeet),
                areaSizeFeet,
                "Fey Presence area size must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(savingThrowAbilityId.Value))
        {
            throw new ArgumentException(
                "Fey Presence saving throw ability ID is required.",
                nameof(savingThrowAbilityId));
        }

        ArgumentNullException.ThrowIfNull(choosableConditionIds);

        ConditionId[] choosableConditionIdArray =
            choosableConditionIds.ToArray();

        if (choosableConditionIdArray.Length < 2)
        {
            throw new ArgumentException(
                "Fey Presence choosable condition IDs must contain at " +
                "least two options.",
                nameof(choosableConditionIds));
        }

        if (!Enum.IsDefined(conditionDurationTrigger))
        {
            throw new ArgumentOutOfRangeException(
                nameof(conditionDurationTrigger),
                conditionDurationTrigger,
                "Fey Presence condition duration trigger must be defined.");
        }

        AreaSizeFeet = areaSizeFeet;
        SavingThrowAbilityId = savingThrowAbilityId;
        ChoosableConditionIds = Array.AsReadOnly(choosableConditionIdArray);
        ConditionDurationTrigger = conditionDurationTrigger;
        RecoversOnShortRest = recoversOnShortRest;
    }

    public int AreaSizeFeet { get; }

    public AbilityId SavingThrowAbilityId { get; }

    public IReadOnlyList<ConditionId> ChoosableConditionIds { get; }

    public NextTurnDurationTrigger ConditionDurationTrigger { get; }

    public bool RecoversOnShortRest { get; }
}
