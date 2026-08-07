namespace FiveEData.Rules.Classes.ShadowStep.Serialization;

internal static class ShadowStepDetailDataMapper
{
    public static ShadowStepDetail Map(ShadowStepDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return new ShadowStepDetail(
            data.TeleportRangeFeet,
            data.GrantsAdvantageOnNextMeleeAttack);
    }
}
