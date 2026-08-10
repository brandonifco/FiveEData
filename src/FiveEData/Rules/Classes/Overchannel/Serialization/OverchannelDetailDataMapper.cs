using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Creatures.DamageTypes;

namespace FiveEData.Rules.Classes.Overchannel.Serialization;

internal static class OverchannelDetailDataMapper
{
    public static OverchannelDetail Map(OverchannelDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        DiceExpressionData selfDamagePerSpellLevelData =
            data.SelfDamagePerSpellLevel
            ?? throw new ArgumentException(
                "Overchannel self damage per spell level is required.",
                nameof(data));

        string selfDamageTypeIdValue = data.SelfDamageTypeId
            ?? throw new ArgumentException(
                "Overchannel self damage type ID is required.",
                nameof(data));

        DiceExpressionData selfDamageIncreaseData =
            data.SelfDamageIncreasePerSubsequentUse
            ?? throw new ArgumentException(
                "Overchannel self damage increase per subsequent use is " +
                "required.",
                nameof(data));

        return new OverchannelDetail(
            data.MaximumSpellLevel,
            data.DealsMaximumDamage,
            data.FirstUseHasNoAdverseEffect,
            new DiceExpression(
                selfDamagePerSpellLevelData.Count,
                selfDamagePerSpellLevelData.Sides),
            new DamageTypeId(selfDamageTypeIdValue),
            new DiceExpression(
                selfDamageIncreaseData.Count,
                selfDamageIncreaseData.Sides),
            data.IgnoresResistanceAndImmunity,
            data.RecoversOnLongRest);
    }
}
