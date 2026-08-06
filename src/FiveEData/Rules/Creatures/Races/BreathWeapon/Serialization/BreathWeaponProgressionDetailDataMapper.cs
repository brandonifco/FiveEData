using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Creatures.Races.BreathWeapon.Serialization;

internal static class BreathWeaponProgressionDetailDataMapper
{
    public static BreathWeaponProgressionDetail Map(
        BreathWeaponProgressionDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        BreathWeaponDamageGrantData[] damageData =
            data.DamageByLevel
            ?? throw new ArgumentException(
                "Breath weapon progression damage by level is required.",
                nameof(data));

        BreathWeaponDamageGrant[] damageByLevel = damageData
            .Select(MapGrant)
            .ToArray();

        return new BreathWeaponProgressionDetail(
            damageByLevel,
            data.RecoversOnShortRest);
    }

    private static BreathWeaponDamageGrant MapGrant(
        BreathWeaponDamageGrantData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        DiceExpressionData damageData =
            data.Damage
            ?? throw new ArgumentException(
                "Breath weapon damage grant damage is required.",
                nameof(data));

        var damage = new DiceExpression(damageData.Count, damageData.Sides);

        return new BreathWeaponDamageGrant(data.CharacterLevel, damage);
    }
}
