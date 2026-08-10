using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Conditions;

namespace FiveEData.Rules.Classes.IntimidatingPresence.Serialization;

internal static class IntimidatingPresenceDetailDataMapper
{
    public static IntimidatingPresenceDetail Map(
        IntimidatingPresenceDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        string savingThrowAbilityIdValue = data.SavingThrowAbilityId
            ?? throw new ArgumentException(
                "Intimidating Presence saving throw ability ID is required.",
                nameof(data));

        string imposedConditionIdValue = data.ImposedConditionId
            ?? throw new ArgumentException(
                "Intimidating Presence imposed condition ID is required.",
                nameof(data));

        return new IntimidatingPresenceDetail(
            data.RangeFeet,
            new AbilityId(savingThrowAbilityIdValue),
            new ConditionId(imposedConditionIdValue),
            data.ConditionDurationTrigger);
    }
}
