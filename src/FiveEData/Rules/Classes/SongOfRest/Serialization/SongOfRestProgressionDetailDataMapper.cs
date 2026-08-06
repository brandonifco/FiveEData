using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Classes.SongOfRest.Serialization;

internal static class SongOfRestProgressionDetailDataMapper
{
    public static SongOfRestProgressionDetail Map(
        SongOfRestProgressionDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        SongOfRestDieGrantData[] dieData =
            data.DieByLevel
            ?? throw new ArgumentException(
                "Song of Rest progression die by level is required.",
                nameof(data));

        SongOfRestDieGrant[] dieByLevel = dieData
            .Select(MapGrant)
            .ToArray();

        return new SongOfRestProgressionDetail(dieByLevel);
    }

    private static SongOfRestDieGrant MapGrant(SongOfRestDieGrantData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        DiceExpressionData dieData =
            data.Die
            ?? throw new ArgumentException(
                "Song of Rest die grant die is required.",
                nameof(data));

        var die = new DiceExpression(dieData.Count, dieData.Sides);

        return new SongOfRestDieGrant(data.CharacterLevel, die);
    }
}
