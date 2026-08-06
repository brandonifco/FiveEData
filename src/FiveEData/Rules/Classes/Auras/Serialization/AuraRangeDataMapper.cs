namespace FiveEData.Rules.Classes.Auras.Serialization;

internal static class AuraRangeDataMapper
{
    public static AuraRange Map(AuraRangeData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return new AuraRange(
            data.BaseRangeFeet,
            data.ExpandedRangeFeet,
            data.ExpandedAtLevel);
    }
}
