namespace FiveEData.Rules.Classes.Auras.Serialization;

internal static class AuraOfWardingDetailDataMapper
{
    public static AuraOfWardingDetail Map(AuraOfWardingDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        AuraRangeData rangeData =
            data.Range
            ?? throw new ArgumentException(
                "Aura of Warding range is required.",
                nameof(data));

        return new AuraOfWardingDetail(
            AuraRangeDataMapper.Map(rangeData),
            data.RequiresConsciousness);
    }
}
