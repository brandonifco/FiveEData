using FiveEData.Rules.Creatures.Abilities;

namespace FiveEData.Rules.Classes.DeathStrike.Serialization;

internal static class DeathStrikeDetailDataMapper
{
    public static DeathStrikeDetail Map(DeathStrikeDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        string savingThrowAbilityIdValue = data.SavingThrowAbilityId
            ?? throw new ArgumentException(
                "Death Strike saving throw ability ID is required.",
                nameof(data));

        return new DeathStrikeDetail(
            new AbilityId(savingThrowAbilityIdValue),
            data.DamageMultiplierOnFailedSave,
            data.RequiresSurprisedTarget);
    }
}
