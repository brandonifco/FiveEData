namespace FiveEData.Rules.Classes.FastMovement.Serialization;

internal static class FastMovementDetailDataMapper
{
    public static FastMovementDetail Map(FastMovementDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return new FastMovementDetail(
            data.SpeedBonusFeet,
            data.RequiresNotWearingHeavyArmor);
    }
}
