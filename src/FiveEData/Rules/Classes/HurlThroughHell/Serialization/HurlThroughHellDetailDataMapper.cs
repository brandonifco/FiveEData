using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Creatures.DamageTypes;

namespace FiveEData.Rules.Classes.HurlThroughHell.Serialization;

internal static class HurlThroughHellDetailDataMapper
{
    public static HurlThroughHellDetail Map(HurlThroughHellDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        DiceExpressionData damageData =
            data.Damage
            ?? throw new ArgumentException(
                "Hurl Through Hell damage is required.",
                nameof(data));

        string damageTypeIdValue =
            data.DamageTypeId
            ?? throw new ArgumentException(
                "Hurl Through Hell damage type ID is required.",
                nameof(data));

        return new HurlThroughHellDetail(
            new DiceExpression(damageData.Count, damageData.Sides),
            new DamageTypeId(damageTypeIdValue),
            data.ExemptsFiends,
            data.RecoversOnLongRest);
    }
}
