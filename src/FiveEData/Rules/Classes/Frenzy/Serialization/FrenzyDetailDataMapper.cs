namespace FiveEData.Rules.Classes.Frenzy.Serialization;

internal static class FrenzyDetailDataMapper
{
    public static FrenzyDetail Map(FrenzyDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return new FrenzyDetail(
            data.GrantsBonusActionMeleeAttack,
            data.ExhaustionLevelsWhenRageEnds);
    }
}
