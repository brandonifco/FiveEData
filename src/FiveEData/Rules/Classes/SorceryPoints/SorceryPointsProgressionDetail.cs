namespace FiveEData.Rules.Classes.SorceryPoints;

public sealed record SorceryPointsProgressionDetail
{
    public SorceryPointsProgressionDetail(
        IEnumerable<SorceryPointsGrant> pointsByLevel,
        bool recoversOnShortRest)
    {
        ArgumentNullException.ThrowIfNull(pointsByLevel);

        PointsByLevel = Array.AsReadOnly(pointsByLevel.ToArray());
        RecoversOnShortRest = recoversOnShortRest;
    }

    public IReadOnlyList<SorceryPointsGrant> PointsByLevel { get; }
    public bool RecoversOnShortRest { get; }
}
