namespace FiveEData.Rules.Classes.DestroyUndead;

public sealed record DestroyUndeadProgressionDetail
{
    public DestroyUndeadProgressionDetail(
        IEnumerable<DestroyUndeadThresholdGrant> thresholdsByLevel)
    {
        ArgumentNullException.ThrowIfNull(thresholdsByLevel);

        ThresholdsByLevel = Array.AsReadOnly(thresholdsByLevel.ToArray());
    }

    public IReadOnlyList<DestroyUndeadThresholdGrant> ThresholdsByLevel
    {
        get;
    }
}
