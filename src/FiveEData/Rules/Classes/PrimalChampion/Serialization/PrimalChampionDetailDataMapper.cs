using FiveEData.Rules.Creatures.Abilities;

namespace FiveEData.Rules.Classes.PrimalChampion.Serialization;

internal static class PrimalChampionDetailDataMapper
{
    public static PrimalChampionDetail Map(PrimalChampionDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        string[] abilityIdValues =
            data.AbilityIds
            ?? throw new ArgumentException(
                "Primal Champion ability IDs are required.",
                nameof(data));

        return new PrimalChampionDetail(
            abilityIdValues.Select(value => new AbilityId(value)),
            data.AbilityScoreIncrease,
            data.MaximumAbilityScore);
    }
}
