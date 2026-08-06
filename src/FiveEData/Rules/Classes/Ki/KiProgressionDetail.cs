namespace FiveEData.Rules.Classes.Ki;

public sealed record KiProgressionDetail
{
    public KiProgressionDetail(
        IEnumerable<KiPointsGrant> pointsByLevel,
        bool recoversOnShortRest)
    {
        ArgumentNullException.ThrowIfNull(pointsByLevel);

        PointsByLevel = Array.AsReadOnly(pointsByLevel.ToArray());
        RecoversOnShortRest = recoversOnShortRest;
    }

    public IReadOnlyList<KiPointsGrant> PointsByLevel { get; }
    public bool RecoversOnShortRest { get; }
}
