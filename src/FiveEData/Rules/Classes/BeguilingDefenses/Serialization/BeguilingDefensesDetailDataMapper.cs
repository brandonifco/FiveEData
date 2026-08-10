using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Conditions;

namespace FiveEData.Rules.Classes.BeguilingDefenses.Serialization;

internal static class BeguilingDefensesDetailDataMapper
{
    public static BeguilingDefensesDetail Map(
        BeguilingDefensesDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        string immuneConditionIdValue = data.ImmuneConditionId
            ?? throw new ArgumentException(
                "Beguiling Defenses immune condition ID is required.",
                nameof(data));

        string reflectionSavingThrowAbilityIdValue =
            data.ReflectionSavingThrowAbilityId
            ?? throw new ArgumentException(
                "Beguiling Defenses reflection saving throw ability ID is " +
                "required.",
                nameof(data));

        return new BeguilingDefensesDetail(
            new ConditionId(immuneConditionIdValue),
            new AbilityId(reflectionSavingThrowAbilityIdValue),
            data.ReflectionDurationMinutes);
    }
}
