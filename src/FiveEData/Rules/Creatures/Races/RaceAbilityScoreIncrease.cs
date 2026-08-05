using FiveEData.Rules.Creatures.Abilities;

namespace FiveEData.Rules.Creatures.Races;

public readonly record struct RaceAbilityScoreIncrease
{
    public RaceAbilityScoreIncrease(AbilityId abilityId, int bonus)
    {
        if (string.IsNullOrWhiteSpace(abilityId.Value))
        {
            throw new ArgumentException(
                "Ability score increase ability ID must not be empty.",
                nameof(abilityId));
        }

        if (bonus <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(bonus),
                bonus,
                "Ability score increase bonus must be greater than zero.");
        }

        AbilityId = abilityId;
        Bonus = bonus;
    }

    public AbilityId AbilityId { get; }

    public int Bonus { get; }
}
