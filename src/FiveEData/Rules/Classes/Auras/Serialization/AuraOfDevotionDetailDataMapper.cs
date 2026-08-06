namespace FiveEData.Rules.Classes.Auras.Serialization;

internal static class AuraOfDevotionDetailDataMapper
{
    public static AuraOfDevotionDetail Map(AuraOfDevotionDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        AuraRangeData rangeData =
            data.Range
            ?? throw new ArgumentException(
                "Aura of Devotion range is required.",
                nameof(data));

        return new AuraOfDevotionDetail(
            AuraRangeDataMapper.Map(rangeData),
            data.RequiresConsciousness);
    }
}
