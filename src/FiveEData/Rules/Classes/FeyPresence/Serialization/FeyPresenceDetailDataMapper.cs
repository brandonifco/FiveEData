using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Conditions;

namespace FiveEData.Rules.Classes.FeyPresence.Serialization;

internal static class FeyPresenceDetailDataMapper
{
    public static FeyPresenceDetail Map(FeyPresenceDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        string savingThrowAbilityIdValue = data.SavingThrowAbilityId
            ?? throw new ArgumentException(
                "Fey Presence saving throw ability ID is required.",
                nameof(data));

        string[] choosableConditionIdValues = data.ChoosableConditionIds
            ?? throw new ArgumentException(
                "Fey Presence choosable condition IDs are required.",
                nameof(data));

        return new FeyPresenceDetail(
            data.AreaSizeFeet,
            new AbilityId(savingThrowAbilityIdValue),
            choosableConditionIdValues.Select(value => new ConditionId(value)),
            data.ConditionDurationTrigger,
            data.RecoversOnShortRest);
    }
}
