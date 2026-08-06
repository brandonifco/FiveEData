namespace FiveEData.Rules.Classes.SorceryPoints.Serialization;

internal static class SorceryPointsProgressionDetailDataMapper
{
    public static SorceryPointsProgressionDetail Map(
        SorceryPointsProgressionDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        SorceryPointsGrantData[] pointsData =
            data.PointsByLevel
            ?? throw new ArgumentException(
                "Sorcery points progression points by level are required.",
                nameof(data));

        SorceryPointsGrant[] pointsByLevel = pointsData
            .Select(
                grant => new SorceryPointsGrant(
                    grant.CharacterLevel,
                    grant.Points))
            .ToArray();

        return new SorceryPointsProgressionDetail(
            pointsByLevel,
            data.RecoversOnShortRest);
    }
}
