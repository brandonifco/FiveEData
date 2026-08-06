namespace FiveEData.Rules.Classes.BardicInspiration;

public sealed record BardicInspirationProgressionDetail
{
    public BardicInspirationProgressionDetail(
        IEnumerable<BardicInspirationDieGrant> dieByLevel,
        int rangeFeet,
        int durationMinutes)
    {
        ArgumentNullException.ThrowIfNull(dieByLevel);

        if (rangeFeet <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(rangeFeet),
                rangeFeet,
                "Range must be greater than zero.");
        }

        if (durationMinutes <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(durationMinutes),
                durationMinutes,
                "Duration must be greater than zero.");
        }

        DieByLevel = Array.AsReadOnly(dieByLevel.ToArray());
        RangeFeet = rangeFeet;
        DurationMinutes = durationMinutes;
    }

    public IReadOnlyList<BardicInspirationDieGrant> DieByLevel { get; }
    public int RangeFeet { get; }
    public int DurationMinutes { get; }
}
