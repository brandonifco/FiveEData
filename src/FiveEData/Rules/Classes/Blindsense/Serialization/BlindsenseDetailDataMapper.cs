namespace FiveEData.Rules.Classes.Blindsense.Serialization;

internal static class BlindsenseDetailDataMapper
{
    public static BlindsenseDetail Map(BlindsenseDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return new BlindsenseDetail(data.RangeFeet, data.RequiresHearing);
    }
}
