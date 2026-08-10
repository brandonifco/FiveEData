namespace FiveEData.Rules.Classes.Assassinate;

public sealed record AssassinateDetail
{
    public AssassinateDetail(
        bool grantsAdvantageAgainstCreaturesThatHaveNotActed,
        bool hitsAgainstSurprisedCreaturesAreCritical)
    {
        GrantsAdvantageAgainstCreaturesThatHaveNotActed =
            grantsAdvantageAgainstCreaturesThatHaveNotActed;
        HitsAgainstSurprisedCreaturesAreCritical =
            hitsAgainstSurprisedCreaturesAreCritical;
    }

    public bool GrantsAdvantageAgainstCreaturesThatHaveNotActed { get; }

    public bool HitsAgainstSurprisedCreaturesAreCritical { get; }
}
