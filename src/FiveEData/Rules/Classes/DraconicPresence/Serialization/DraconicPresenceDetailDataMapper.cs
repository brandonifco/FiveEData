using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Conditions;

namespace FiveEData.Rules.Classes.DraconicPresence.Serialization;

internal static class DraconicPresenceDetailDataMapper
{
    public static DraconicPresenceDetail Map(
        DraconicPresenceDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        string[] choosableConditionIdValues = data.ChoosableConditionIds
            ?? throw new ArgumentException(
                "Draconic Presence choosable condition IDs are required.",
                nameof(data));

        string savingThrowAbilityIdValue = data.SavingThrowAbilityId
            ?? throw new ArgumentException(
                "Draconic Presence saving throw ability ID is required.",
                nameof(data));

        return new DraconicPresenceDetail(
            data.SorceryPointCost,
            data.RangeFeet,
            choosableConditionIdValues.Select(value => new ConditionId(value)),
            new AbilityId(savingThrowAbilityIdValue),
            data.DurationMinutes,
            data.RequiresConcentration);
    }
}
