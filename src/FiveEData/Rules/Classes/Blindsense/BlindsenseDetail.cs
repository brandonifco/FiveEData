namespace FiveEData.Rules.Classes.Blindsense;

public sealed record BlindsenseDetail
{
    public BlindsenseDetail(int rangeFeet, bool requiresHearing)
    {
        if (rangeFeet <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(rangeFeet),
                rangeFeet,
                "Blindsense range must be greater than zero.");
        }

        RangeFeet = rangeFeet;
        RequiresHearing = requiresHearing;
    }

    public int RangeFeet { get; }

    public bool RequiresHearing { get; }
}
