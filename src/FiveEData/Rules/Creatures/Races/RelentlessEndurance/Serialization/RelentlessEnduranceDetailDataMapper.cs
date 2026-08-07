namespace FiveEData.Rules.Creatures.Races.RelentlessEndurance.Serialization;

internal static class RelentlessEnduranceDetailDataMapper
{
    public static RelentlessEnduranceDetail Map(
        RelentlessEnduranceDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return new RelentlessEnduranceDetail(
            data.HitPointsRetained,
            data.RecoversOnLongRest);
    }
}
