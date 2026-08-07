namespace FiveEData.Rules.Classes.FeralSenses;

public sealed record FeralSensesDetail
{
    public FeralSensesDetail(
        int rangeFeet,
        bool negatesUnseenAttackDisadvantage)
    {
        if (rangeFeet <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(rangeFeet),
                rangeFeet,
                "Feral Senses range must be greater than zero.");
        }

        RangeFeet = rangeFeet;
        NegatesUnseenAttackDisadvantage = negatesUnseenAttackDisadvantage;
    }

    public int RangeFeet { get; }

    public bool NegatesUnseenAttackDisadvantage { get; }
}
