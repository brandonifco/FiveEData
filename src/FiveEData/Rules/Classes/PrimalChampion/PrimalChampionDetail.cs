using FiveEData.Rules.Creatures.Abilities;

namespace FiveEData.Rules.Classes.PrimalChampion;

public sealed record PrimalChampionDetail
{
    public PrimalChampionDetail(
        IEnumerable<AbilityId> abilityIds,
        int abilityScoreIncrease,
        int maximumAbilityScore)
    {
        ArgumentNullException.ThrowIfNull(abilityIds);

        if (abilityScoreIncrease <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(abilityScoreIncrease),
                abilityScoreIncrease,
                "Primal Champion ability score increase must be greater " +
                "than zero.");
        }

        if (maximumAbilityScore <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maximumAbilityScore),
                maximumAbilityScore,
                "Primal Champion maximum ability score must be greater " +
                "than zero.");
        }

        AbilityIds = Array.AsReadOnly(abilityIds.ToArray());
        AbilityScoreIncrease = abilityScoreIncrease;
        MaximumAbilityScore = maximumAbilityScore;
    }

    public IReadOnlyList<AbilityId> AbilityIds { get; }

    public int AbilityScoreIncrease { get; }

    public int MaximumAbilityScore { get; }
}
