using FiveEData.Rules.Common;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.DamageTypes;

namespace FiveEData.Rules.Classes.WrathOfTheStorm;

public sealed record WrathOfTheStormDetail
{
    public WrathOfTheStormDetail(
        int triggerRangeFeet,
        DiceExpression damage,
        IEnumerable<DamageTypeId> choosableDamageTypeIds,
        AbilityId savingThrowAbilityId,
        bool halfDamageOnSuccessfulSave,
        bool recoversOnLongRest)
    {
        ArgumentNullException.ThrowIfNull(choosableDamageTypeIds);

        if (triggerRangeFeet <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(triggerRangeFeet),
                triggerRangeFeet,
                "Wrath of the Storm trigger range must be greater than " +
                "zero.");
        }

        if (string.IsNullOrWhiteSpace(savingThrowAbilityId.Value))
        {
            throw new ArgumentException(
                "Wrath of the Storm saving throw ability ID is required.",
                nameof(savingThrowAbilityId));
        }

        TriggerRangeFeet = triggerRangeFeet;
        Damage = damage;
        ChoosableDamageTypeIds =
            Array.AsReadOnly(choosableDamageTypeIds.ToArray());
        SavingThrowAbilityId = savingThrowAbilityId;
        HalfDamageOnSuccessfulSave = halfDamageOnSuccessfulSave;
        RecoversOnLongRest = recoversOnLongRest;
    }

    public int TriggerRangeFeet { get; }

    public DiceExpression Damage { get; }

    public IReadOnlyList<DamageTypeId> ChoosableDamageTypeIds { get; }

    public AbilityId SavingThrowAbilityId { get; }

    public bool HalfDamageOnSuccessfulSave { get; }

    public bool RecoversOnLongRest { get; }
}
