namespace FiveEData.Rules.Creatures.Races.Lucky;

public sealed record LuckyDetail
{
    public LuckyDetail(int rerollOnNaturalRoll, bool mustUseNewRoll)
    {
        if (rerollOnNaturalRoll is < 1 or > 20)
        {
            throw new ArgumentOutOfRangeException(
                nameof(rerollOnNaturalRoll),
                rerollOnNaturalRoll,
                "Lucky reroll trigger must be between 1 and 20.");
        }

        RerollOnNaturalRoll = rerollOnNaturalRoll;
        MustUseNewRoll = mustUseNewRoll;
    }

    public int RerollOnNaturalRoll { get; }

    public bool MustUseNewRoll { get; }
}
