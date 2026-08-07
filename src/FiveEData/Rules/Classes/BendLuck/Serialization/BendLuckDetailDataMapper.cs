using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Classes.BendLuck.Serialization;

internal static class BendLuckDetailDataMapper
{
    public static BendLuckDetail Map(BendLuckDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        DiceExpressionData dieData =
            data.Die
            ?? throw new ArgumentException(
                "Bend Luck die is required.",
                nameof(data));

        return new BendLuckDetail(
            data.SorceryPointCost,
            new DiceExpression(dieData.Count, dieData.Sides));
    }
}
