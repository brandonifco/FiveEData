using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Conditions;

namespace FiveEData.Rules.Classes.DarkDelirium.Serialization;

internal static class DarkDeliriumDetailDataMapper
{
    public static DarkDeliriumDetail Map(DarkDeliriumDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        string savingThrowAbilityIdValue = data.SavingThrowAbilityId
            ?? throw new ArgumentException(
                "Dark Delirium saving throw ability ID is required.",
                nameof(data));

        string[] choosableConditionIdValues = data.ChoosableConditionIds
            ?? throw new ArgumentException(
                "Dark Delirium choosable condition IDs are required.",
                nameof(data));

        return new DarkDeliriumDetail(
            data.RangeFeet,
            new AbilityId(savingThrowAbilityIdValue),
            choosableConditionIdValues.Select(value => new ConditionId(value)),
            data.DurationMinutes,
            data.RequiresConcentration,
            data.RecoversOnShortRest);
    }
}
