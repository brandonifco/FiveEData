using FiveEData.Rules.Common;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Conditions;

namespace FiveEData.Rules.Classes.IntimidatingPresence;

public sealed record IntimidatingPresenceDetail
{
    public IntimidatingPresenceDetail(
        int rangeFeet,
        AbilityId savingThrowAbilityId,
        ConditionId imposedConditionId,
        NextTurnDurationTrigger conditionDurationTrigger)
    {
        if (rangeFeet <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(rangeFeet),
                rangeFeet,
                "Intimidating Presence range must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(savingThrowAbilityId.Value))
        {
            throw new ArgumentException(
                "Intimidating Presence saving throw ability ID is required.",
                nameof(savingThrowAbilityId));
        }

        if (string.IsNullOrWhiteSpace(imposedConditionId.Value))
        {
            throw new ArgumentException(
                "Intimidating Presence imposed condition ID is required.",
                nameof(imposedConditionId));
        }

        if (!Enum.IsDefined(conditionDurationTrigger))
        {
            throw new ArgumentOutOfRangeException(
                nameof(conditionDurationTrigger),
                conditionDurationTrigger,
                "Intimidating Presence condition duration trigger must be " +
                "defined.");
        }

        RangeFeet = rangeFeet;
        SavingThrowAbilityId = savingThrowAbilityId;
        ImposedConditionId = imposedConditionId;
        ConditionDurationTrigger = conditionDurationTrigger;
    }

    public int RangeFeet { get; }

    public AbilityId SavingThrowAbilityId { get; }

    public ConditionId ImposedConditionId { get; }

    public NextTurnDurationTrigger ConditionDurationTrigger { get; }
}
