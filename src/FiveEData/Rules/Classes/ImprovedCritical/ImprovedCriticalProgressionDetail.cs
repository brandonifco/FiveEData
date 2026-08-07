namespace FiveEData.Rules.Classes.ImprovedCritical;

public sealed record ImprovedCriticalProgressionDetail
{
    public ImprovedCriticalProgressionDetail(
        IEnumerable<CriticalHitThresholdGrant> minimumRollByLevel)
    {
        ArgumentNullException.ThrowIfNull(minimumRollByLevel);

        MinimumRollByLevel = Array.AsReadOnly(minimumRollByLevel.ToArray());
    }

    public IReadOnlyList<CriticalHitThresholdGrant> MinimumRollByLevel
    {
        get;
    }
}
