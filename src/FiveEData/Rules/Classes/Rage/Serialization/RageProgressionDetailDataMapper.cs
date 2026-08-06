using FiveEData.Rules.Creatures.DamageTypes;

namespace FiveEData.Rules.Classes.Rage.Serialization;

internal static class RageProgressionDetailDataMapper
{
    public static RageProgressionDetail Map(RageProgressionDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        RageUseGrantData[] usesData =
            data.UsesByLevel
            ?? throw new ArgumentException(
                "Rage progression uses by level are required.",
                nameof(data));

        RageDamageBonusGrantData[] damageBonusData =
            data.DamageBonusByLevel
            ?? throw new ArgumentException(
                "Rage progression damage bonus by level is required.",
                nameof(data));

        string[] resistedDamageTypeIdValues =
            data.ResistedDamageTypeIds
            ?? throw new ArgumentException(
                "Rage progression resisted damage type IDs are " +
                "required.",
                nameof(data));

        RageUseGrant[] usesByLevel = usesData
            .Select(
                grant => new RageUseGrant(
                    grant.CharacterLevel,
                    grant.UsesPerLongRest))
            .ToArray();

        RageDamageBonusGrant[] damageBonusByLevel = damageBonusData
            .Select(
                grant => new RageDamageBonusGrant(
                    grant.CharacterLevel,
                    grant.Bonus))
            .ToArray();

        DamageTypeId[] resistedDamageTypeIds = resistedDamageTypeIdValues
            .Select(value => new DamageTypeId(value))
            .ToArray();

        return new RageProgressionDetail(
            usesByLevel,
            damageBonusByLevel,
            data.DurationMinutes,
            resistedDamageTypeIds,
            data.RequiresNotWearingHeavyArmor);
    }
}
