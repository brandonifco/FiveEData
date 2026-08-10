namespace FiveEData.Rules.Classes.AwakenedMind.Serialization;

internal static class AwakenedMindDetailDataMapper
{
    public static AwakenedMindDetail Map(AwakenedMindDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return new AwakenedMindDetail(data.TelepathyRangeFeet);
    }
}
