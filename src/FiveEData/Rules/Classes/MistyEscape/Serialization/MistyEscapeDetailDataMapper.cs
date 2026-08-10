namespace FiveEData.Rules.Classes.MistyEscape.Serialization;

internal static class MistyEscapeDetailDataMapper
{
    public static MistyEscapeDetail Map(MistyEscapeDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return new MistyEscapeDetail(
            data.TeleportRangeFeet,
            data.GrantsInvisibility,
            data.RecoversOnShortRest);
    }
}
