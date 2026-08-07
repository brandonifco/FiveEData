namespace FiveEData.Rules.Classes.DestroyUndead.Serialization;

internal static class DestroyUndeadProgressionDetailDataMapper
{
    public static DestroyUndeadProgressionDetail Map(
        DestroyUndeadProgressionDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        DestroyUndeadThresholdGrantData[] thresholdData =
            data.ThresholdsByLevel
            ?? throw new ArgumentException(
                "Destroy Undead progression thresholds by level is required.",
                nameof(data));

        DestroyUndeadThresholdGrant[] thresholdsByLevel = thresholdData
            .Select(MapGrant)
            .ToArray();

        return new DestroyUndeadProgressionDetail(thresholdsByLevel);
    }

    private static DestroyUndeadThresholdGrant MapGrant(
        DestroyUndeadThresholdGrantData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return new DestroyUndeadThresholdGrant(
            data.CharacterLevel,
            data.MaxChallengeRating);
    }
}
