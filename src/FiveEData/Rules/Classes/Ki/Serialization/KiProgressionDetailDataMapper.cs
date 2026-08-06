namespace FiveEData.Rules.Classes.Ki.Serialization;

internal static class KiProgressionDetailDataMapper
{
    public static KiProgressionDetail Map(KiProgressionDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        KiPointsGrantData[] pointsData =
            data.PointsByLevel
            ?? throw new ArgumentException(
                "Ki progression points by level are required.",
                nameof(data));

        KiPointsGrant[] pointsByLevel = pointsData
            .Select(
                grant => new KiPointsGrant(
                    grant.CharacterLevel,
                    grant.Points))
            .ToArray();

        return new KiProgressionDetail(
            pointsByLevel,
            data.RecoversOnShortRest);
    }
}
