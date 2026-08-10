using FiveEData.Rules.Creatures.Abilities;

namespace FiveEData.Rules.Classes.DeathStrike;

public sealed record DeathStrikeDetail
{
    public DeathStrikeDetail(
        AbilityId savingThrowAbilityId,
        int damageMultiplierOnFailedSave,
        bool requiresSurprisedTarget)
    {
        if (string.IsNullOrWhiteSpace(savingThrowAbilityId.Value))
        {
            throw new ArgumentException(
                "Death Strike saving throw ability ID is required.",
                nameof(savingThrowAbilityId));
        }

        if (damageMultiplierOnFailedSave <= 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(damageMultiplierOnFailedSave),
                damageMultiplierOnFailedSave,
                "Death Strike damage multiplier on a failed save must be " +
                "greater than one.");
        }

        SavingThrowAbilityId = savingThrowAbilityId;
        DamageMultiplierOnFailedSave = damageMultiplierOnFailedSave;
        RequiresSurprisedTarget = requiresSurprisedTarget;
    }

    public AbilityId SavingThrowAbilityId { get; }

    public int DamageMultiplierOnFailedSave { get; }

    public bool RequiresSurprisedTarget { get; }
}
