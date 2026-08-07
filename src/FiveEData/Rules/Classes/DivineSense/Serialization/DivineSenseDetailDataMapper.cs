namespace FiveEData.Rules.Classes.DivineSense.Serialization;

internal static class DivineSenseDetailDataMapper
{
    public static DivineSenseDetail Map(DivineSenseDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return new DivineSenseDetail(
            data.RangeFeet,
            data.RecoversOnLongRest);
    }
}
