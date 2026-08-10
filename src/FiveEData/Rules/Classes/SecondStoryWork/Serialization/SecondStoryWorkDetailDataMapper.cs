namespace FiveEData.Rules.Classes.SecondStoryWork.Serialization;

internal static class SecondStoryWorkDetailDataMapper
{
    public static SecondStoryWorkDetail Map(SecondStoryWorkDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return new SecondStoryWorkDetail(
            data.ClimbingCostsNoExtraMovement,
            data.AddsDexterityModifierToRunningJumpDistance);
    }
}
