using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Conditions;

namespace FiveEData.Rules.Classes.BeguilingDefenses;

public sealed record BeguilingDefensesDetail
{
    public BeguilingDefensesDetail(
        ConditionId immuneConditionId,
        AbilityId reflectionSavingThrowAbilityId,
        int reflectionDurationMinutes)
    {
        if (string.IsNullOrWhiteSpace(immuneConditionId.Value))
        {
            throw new ArgumentException(
                "Beguiling Defenses immune condition ID is required.",
                nameof(immuneConditionId));
        }

        if (string.IsNullOrWhiteSpace(reflectionSavingThrowAbilityId.Value))
        {
            throw new ArgumentException(
                "Beguiling Defenses reflection saving throw ability ID is " +
                "required.",
                nameof(reflectionSavingThrowAbilityId));
        }

        if (reflectionDurationMinutes <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(reflectionDurationMinutes),
                reflectionDurationMinutes,
                "Beguiling Defenses reflection duration must be greater " +
                "than zero.");
        }

        ImmuneConditionId = immuneConditionId;
        ReflectionSavingThrowAbilityId = reflectionSavingThrowAbilityId;
        ReflectionDurationMinutes = reflectionDurationMinutes;
    }

    public ConditionId ImmuneConditionId { get; }

    public AbilityId ReflectionSavingThrowAbilityId { get; }

    public int ReflectionDurationMinutes { get; }
}
