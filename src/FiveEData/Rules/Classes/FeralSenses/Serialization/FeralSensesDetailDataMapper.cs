namespace FiveEData.Rules.Classes.FeralSenses.Serialization;

internal static class FeralSensesDetailDataMapper
{
    public static FeralSensesDetail Map(FeralSensesDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return new FeralSensesDetail(
            data.RangeFeet,
            data.NegatesUnseenAttackDisadvantage);
    }
}
