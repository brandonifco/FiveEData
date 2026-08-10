using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.DamageTypes;

namespace FiveEData.Rules.Classes.WrathOfTheStorm.Serialization;

internal static class WrathOfTheStormDetailDataMapper
{
    public static WrathOfTheStormDetail Map(WrathOfTheStormDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        DiceExpressionData damageData =
            data.Damage
            ?? throw new ArgumentException(
                "Wrath of the Storm damage is required.",
                nameof(data));

        string[] damageTypeIdValues =
            data.ChoosableDamageTypeIds
            ?? throw new ArgumentException(
                "Wrath of the Storm choosable damage type IDs are required.",
                nameof(data));

        string savingThrowAbilityIdValue =
            data.SavingThrowAbilityId
            ?? throw new ArgumentException(
                "Wrath of the Storm saving throw ability ID is required.",
                nameof(data));

        AbilityModifierUsesGrantData usesPerRestData =
            data.UsesPerRest
            ?? throw new ArgumentException(
                "Wrath of the Storm uses per rest is required.",
                nameof(data));

        return new WrathOfTheStormDetail(
            data.TriggerRangeFeet,
            new DiceExpression(damageData.Count, damageData.Sides),
            damageTypeIdValues.Select(value => new DamageTypeId(value)),
            new AbilityId(savingThrowAbilityIdValue),
            data.HalfDamageOnSuccessfulSave,
            AbilityModifierUsesGrantDataMapper.Map(usesPerRestData));
    }
}
