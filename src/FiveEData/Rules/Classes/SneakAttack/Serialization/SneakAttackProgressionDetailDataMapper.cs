using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Classes.SneakAttack.Serialization;

internal static class SneakAttackProgressionDetailDataMapper
{
    public static SneakAttackProgressionDetail Map(
        SneakAttackProgressionDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        SneakAttackDiceGrantData[] diceData =
            data.DiceByLevel
            ?? throw new ArgumentException(
                "Sneak Attack progression dice by level are required.",
                nameof(data));

        SneakAttackDiceGrant[] diceByLevel = diceData
            .Select(MapGrant)
            .ToArray();

        return new SneakAttackProgressionDetail(
            diceByLevel,
            data.OncePerTurn,
            data.RequiresFinesseOrRangedWeapon);
    }

    private static SneakAttackDiceGrant MapGrant(SneakAttackDiceGrantData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        DiceExpressionData damageData =
            data.Damage
            ?? throw new ArgumentException(
                "Sneak Attack dice grant damage is required.",
                nameof(data));

        var damage = new DiceExpression(damageData.Count, damageData.Sides);

        return new SneakAttackDiceGrant(data.CharacterLevel, damage);
    }
}
