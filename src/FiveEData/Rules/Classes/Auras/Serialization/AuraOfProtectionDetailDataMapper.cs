namespace FiveEData.Rules.Classes.Auras.Serialization;

internal static class AuraOfProtectionDetailDataMapper
{
    public static AuraOfProtectionDetail Map(AuraOfProtectionDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        AuraRangeData rangeData =
            data.Range
            ?? throw new ArgumentException(
                "Aura of Protection range is required.",
                nameof(data));

        return new AuraOfProtectionDetail(
            AuraRangeDataMapper.Map(rangeData),
            data.RequiresConsciousness,
            data.SavingThrowBonusMinimum);
    }
}
