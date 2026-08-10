using FiveEData.Rules.Creatures.Abilities;

namespace FiveEData.Rules.Common;

public readonly record struct AbilityModifierUsesGrant
{
    public AbilityModifierUsesGrant(
        AbilityId abilityId,
        bool recoversOnLongRest)
    {
        if (string.IsNullOrWhiteSpace(abilityId.Value))
        {
            throw new ArgumentException(
                "Ability modifier uses grant ability ID is required.",
                nameof(abilityId));
        }

        AbilityId = abilityId;
        RecoversOnLongRest = recoversOnLongRest;
    }

    public AbilityId AbilityId { get; }
    public bool RecoversOnLongRest { get; }
}
