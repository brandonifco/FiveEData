namespace FiveEData.Rules.Creatures.Races.Lucky.Serialization;

internal static class LuckyDetailDataMapper
{
    public static LuckyDetail Map(LuckyDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return new LuckyDetail(
            data.RerollOnNaturalRoll,
            data.MustUseNewRoll);
    }
}
