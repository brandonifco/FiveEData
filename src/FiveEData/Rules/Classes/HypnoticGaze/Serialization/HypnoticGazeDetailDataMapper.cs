using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Conditions;

namespace FiveEData.Rules.Classes.HypnoticGaze.Serialization;

internal static class HypnoticGazeDetailDataMapper
{
    public static HypnoticGazeDetail Map(HypnoticGazeDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        string savingThrowAbilityIdValue = data.SavingThrowAbilityId
            ?? throw new ArgumentException(
                "Hypnotic Gaze saving throw ability ID is required.",
                nameof(data));

        string[] imposedConditionIdValues = data.ImposedConditionIds
            ?? throw new ArgumentException(
                "Hypnotic Gaze imposed condition IDs are required.",
                nameof(data));

        return new HypnoticGazeDetail(
            data.RangeFeet,
            new AbilityId(savingThrowAbilityIdValue),
            imposedConditionIdValues.Select(value => new ConditionId(value)),
            data.SetsSpeedToZero,
            data.ConditionDurationTrigger);
    }
}
