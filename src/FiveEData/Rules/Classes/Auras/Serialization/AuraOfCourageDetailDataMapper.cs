namespace FiveEData.Rules.Classes.Auras.Serialization;

internal static class AuraOfCourageDetailDataMapper
{
    public static AuraOfCourageDetail Map(AuraOfCourageDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        AuraRangeData rangeData =
            data.Range
            ?? throw new ArgumentException(
                "Aura of Courage range is required.",
                nameof(data));

        return new AuraOfCourageDetail(
            AuraRangeDataMapper.Map(rangeData),
            data.RequiresConsciousness);
    }
}
