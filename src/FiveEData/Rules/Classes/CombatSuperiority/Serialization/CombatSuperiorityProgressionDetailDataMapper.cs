using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Classes.CombatSuperiority.Serialization;

internal static class CombatSuperiorityProgressionDetailDataMapper
{
    public static CombatSuperiorityProgressionDetail Map(
        CombatSuperiorityProgressionDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        CombatSuperiorityManeuversKnownGrantData[] maneuversKnownData =
            data.ManeuversKnownByLevel
            ?? throw new ArgumentException(
                "Combat Superiority progression maneuvers known by level " +
                "is required.",
                nameof(data));

        CombatSuperiorityDiceCountGrantData[] diceCountData =
            data.DiceCountByLevel
            ?? throw new ArgumentException(
                "Combat Superiority progression dice count by level is " +
                "required.",
                nameof(data));

        CombatSuperiorityDieSizeGrantData[] dieSizeData =
            data.DieSizeByLevel
            ?? throw new ArgumentException(
                "Combat Superiority progression die size by level is " +
                "required.",
                nameof(data));

        return new CombatSuperiorityProgressionDetail(
            maneuversKnownData.Select(MapManeuversKnownGrant),
            diceCountData.Select(MapDiceCountGrant),
            dieSizeData.Select(MapDieSizeGrant));
    }

    private static CombatSuperiorityManeuversKnownGrant
        MapManeuversKnownGrant(CombatSuperiorityManeuversKnownGrantData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return new CombatSuperiorityManeuversKnownGrant(
            data.CharacterLevel,
            data.ManeuversKnown);
    }

    private static CombatSuperiorityDiceCountGrant MapDiceCountGrant(
        CombatSuperiorityDiceCountGrantData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return new CombatSuperiorityDiceCountGrant(
            data.CharacterLevel,
            data.DiceCount);
    }

    private static CombatSuperiorityDieSizeGrant MapDieSizeGrant(
        CombatSuperiorityDieSizeGrantData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        DiceExpressionData dieData =
            data.Die
            ?? throw new ArgumentException(
                "Combat Superiority die size grant die is required.",
                nameof(data));

        var die = new DiceExpression(dieData.Count, dieData.Sides);

        return new CombatSuperiorityDieSizeGrant(data.CharacterLevel, die);
    }
}
