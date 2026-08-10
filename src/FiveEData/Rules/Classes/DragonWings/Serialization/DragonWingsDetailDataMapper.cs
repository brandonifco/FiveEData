namespace FiveEData.Rules.Classes.DragonWings.Serialization;

internal static class DragonWingsDetailDataMapper
{
    public static DragonWingsDetail Map(DragonWingsDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return new DragonWingsDetail(
            data.GrantsFlyingSpeedEqualToCurrentSpeed,
            data.RequiresBonusActionToCreateOrDismiss);
    }
}
