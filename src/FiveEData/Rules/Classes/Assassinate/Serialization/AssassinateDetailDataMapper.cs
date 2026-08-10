namespace FiveEData.Rules.Classes.Assassinate.Serialization;

internal static class AssassinateDetailDataMapper
{
    public static AssassinateDetail Map(AssassinateDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return new AssassinateDetail(
            data.GrantsAdvantageAgainstCreaturesThatHaveNotActed,
            data.HitsAgainstSurprisedCreaturesAreCritical);
    }
}
