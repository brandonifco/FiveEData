using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Classes.BardicInspiration.Serialization;

internal static class BardicInspirationProgressionDetailDataMapper
{
    public static BardicInspirationProgressionDetail Map(
        BardicInspirationProgressionDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        BardicInspirationDieGrantData[] dieData =
            data.DieByLevel
            ?? throw new ArgumentException(
                "Bardic Inspiration progression die by level is required.",
                nameof(data));

        BardicInspirationDieGrant[] dieByLevel = dieData
            .Select(MapGrant)
            .ToArray();

        return new BardicInspirationProgressionDetail(
            dieByLevel,
            data.RangeFeet,
            data.DurationMinutes);
    }

    private static BardicInspirationDieGrant MapGrant(
        BardicInspirationDieGrantData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        DiceExpressionData dieData =
            data.Die
            ?? throw new ArgumentException(
                "Bardic Inspiration die grant die is required.",
                nameof(data));

        var die = new DiceExpression(dieData.Count, dieData.Sides);

        return new BardicInspirationDieGrant(data.CharacterLevel, die);
    }
}
