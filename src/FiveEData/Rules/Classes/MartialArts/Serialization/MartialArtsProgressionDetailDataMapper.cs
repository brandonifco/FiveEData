using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Classes.MartialArts.Serialization;

internal static class MartialArtsProgressionDetailDataMapper
{
    public static MartialArtsProgressionDetail Map(
        MartialArtsProgressionDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        MartialArtsDieGrantData[] dieData =
            data.DieByLevel
            ?? throw new ArgumentException(
                "Martial Arts progression die by level is required.",
                nameof(data));

        MartialArtsDieGrant[] dieByLevel = dieData
            .Select(MapGrant)
            .ToArray();

        return new MartialArtsProgressionDetail(
            dieByLevel,
            data.CanUseDexterityForAttackAndDamage,
            data.GrantsBonusActionUnarmedStrike,
            data.RequiresNotWearingArmor,
            data.RequiresNotWieldingShield);
    }

    private static MartialArtsDieGrant MapGrant(MartialArtsDieGrantData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        DiceExpressionData dieData =
            data.Die
            ?? throw new ArgumentException(
                "Martial Arts die grant die is required.",
                nameof(data));

        var die = new DiceExpression(dieData.Count, dieData.Sides);

        return new MartialArtsDieGrant(data.CharacterLevel, die);
    }
}
