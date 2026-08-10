using FiveEData.Rules.Common;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Conditions;

namespace FiveEData.Rules.Classes.HypnoticGaze;

public sealed record HypnoticGazeDetail
{
    public HypnoticGazeDetail(
        int rangeFeet,
        AbilityId savingThrowAbilityId,
        IEnumerable<ConditionId> imposedConditionIds,
        bool setsSpeedToZero,
        NextTurnDurationTrigger conditionDurationTrigger)
    {
        if (rangeFeet <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(rangeFeet),
                rangeFeet,
                "Hypnotic Gaze range must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(savingThrowAbilityId.Value))
        {
            throw new ArgumentException(
                "Hypnotic Gaze saving throw ability ID is required.",
                nameof(savingThrowAbilityId));
        }

        ArgumentNullException.ThrowIfNull(imposedConditionIds);

        ConditionId[] imposedConditionIdArray = imposedConditionIds.ToArray();

        if (imposedConditionIdArray.Length == 0)
        {
            throw new ArgumentException(
                "Hypnotic Gaze imposed condition IDs must not be empty.",
                nameof(imposedConditionIds));
        }

        if (!Enum.IsDefined(conditionDurationTrigger))
        {
            throw new ArgumentOutOfRangeException(
                nameof(conditionDurationTrigger),
                conditionDurationTrigger,
                "Hypnotic Gaze condition duration trigger must be defined.");
        }

        RangeFeet = rangeFeet;
        SavingThrowAbilityId = savingThrowAbilityId;
        ImposedConditionIds = Array.AsReadOnly(imposedConditionIdArray);
        SetsSpeedToZero = setsSpeedToZero;
        ConditionDurationTrigger = conditionDurationTrigger;
    }

    public int RangeFeet { get; }

    public AbilityId SavingThrowAbilityId { get; }

    public IReadOnlyList<ConditionId> ImposedConditionIds { get; }

    public bool SetsSpeedToZero { get; }

    public NextTurnDurationTrigger ConditionDurationTrigger { get; }
}
