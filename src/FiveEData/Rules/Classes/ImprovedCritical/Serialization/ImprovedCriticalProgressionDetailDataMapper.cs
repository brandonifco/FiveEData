namespace FiveEData.Rules.Classes.ImprovedCritical.Serialization;

internal static class ImprovedCriticalProgressionDetailDataMapper
{
    public static ImprovedCriticalProgressionDetail Map(
        ImprovedCriticalProgressionDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        CriticalHitThresholdGrantData[] grantData =
            data.MinimumRollByLevel
            ?? throw new ArgumentException(
                "Improved Critical progression minimum roll by level is " +
                "required.",
                nameof(data));

        CriticalHitThresholdGrant[] minimumRollByLevel = grantData
            .Select(MapGrant)
            .ToArray();

        return new ImprovedCriticalProgressionDetail(minimumRollByLevel);
    }

    private static CriticalHitThresholdGrant MapGrant(
        CriticalHitThresholdGrantData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return new CriticalHitThresholdGrant(
            data.CharacterLevel,
            data.MinimumRoll);
    }
}
