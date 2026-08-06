using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Creatures.DamageTypes;

namespace FiveEData.Rules.Classes.DivineStrike.Serialization;

internal static class DivineStrikeProgressionDetailDataMapper
{
    public static DivineStrikeProgressionDetail Map(
        DivineStrikeProgressionDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        DivineStrikeDamageGrantData[] damageData =
            data.DamageByLevel
            ?? throw new ArgumentException(
                "Divine Strike progression damage by level is required.",
                nameof(data));

        DivineStrikeDamageGrant[] damageByLevel = damageData
            .Select(MapGrant)
            .ToArray();

        DamageTypeId? fixedDamageTypeId =
            data.FixedDamageTypeId is { } fixedDamageTypeIdValue
                ? new DamageTypeId(fixedDamageTypeIdValue)
                : null;

        DamageTypeId[]? choosableDamageTypeIds =
            data.ChoosableDamageTypeIds?
                .Select(value => new DamageTypeId(value))
                .ToArray();

        return new DivineStrikeProgressionDetail(
            damageByLevel,
            fixedDamageTypeId,
            choosableDamageTypeIds,
            data.MatchesWeaponDamageType);
    }

    private static DivineStrikeDamageGrant MapGrant(
        DivineStrikeDamageGrantData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        DiceExpressionData damageData =
            data.Damage
            ?? throw new ArgumentException(
                "Divine Strike damage grant damage is required.",
                nameof(data));

        var damage = new DiceExpression(damageData.Count, damageData.Sides);

        return new DivineStrikeDamageGrant(data.CharacterLevel, damage);
    }
}
