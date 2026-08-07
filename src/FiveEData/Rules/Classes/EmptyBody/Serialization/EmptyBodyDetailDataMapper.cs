namespace FiveEData.Rules.Classes.EmptyBody.Serialization;

internal static class EmptyBodyDetailDataMapper
{
    public static EmptyBodyDetail Map(EmptyBodyDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return new EmptyBodyDetail(
            data.InvisibilityKiCost,
            data.InvisibilityDurationMinutes,
            data.AstralProjectionKiCost);
    }
}
