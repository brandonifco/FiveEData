namespace FiveEData.Rules.Classes.DivineSense;

public sealed record DivineSenseDetail
{
    public DivineSenseDetail(int rangeFeet, bool recoversOnLongRest)
    {
        if (rangeFeet <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(rangeFeet),
                rangeFeet,
                "Divine Sense range must be greater than zero.");
        }

        RangeFeet = rangeFeet;
        RecoversOnLongRest = recoversOnLongRest;
    }

    public int RangeFeet { get; }

    public bool RecoversOnLongRest { get; }
}
