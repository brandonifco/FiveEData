namespace FiveEData.Rules.Classes.UnarmoredMovement.Serialization;

internal static class UnarmoredMovementProgressionDetailDataMapper
{
    public static UnarmoredMovementProgressionDetail Map(
        UnarmoredMovementProgressionDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        UnarmoredMovementSpeedBonusGrantData[] speedBonusData =
            data.SpeedBonusByLevel
            ?? throw new ArgumentException(
                "Unarmored Movement progression speed bonus by level is " +
                "required.",
                nameof(data));

        UnarmoredMovementSpeedBonusGrant[] speedBonusByLevel = speedBonusData
            .Select(MapGrant)
            .ToArray();

        return new UnarmoredMovementProgressionDetail(
            speedBonusByLevel,
            data.RequiresNotWearingArmor,
            data.RequiresNotWieldingShield);
    }

    private static UnarmoredMovementSpeedBonusGrant MapGrant(
        UnarmoredMovementSpeedBonusGrantData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return new UnarmoredMovementSpeedBonusGrant(
            data.CharacterLevel,
            data.SpeedBonusFeet);
    }
}
