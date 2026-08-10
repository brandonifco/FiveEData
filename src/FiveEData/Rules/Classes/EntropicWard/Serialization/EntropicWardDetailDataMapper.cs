namespace FiveEData.Rules.Classes.EntropicWard.Serialization;

internal static class EntropicWardDetailDataMapper
{
    public static EntropicWardDetail Map(EntropicWardDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return new EntropicWardDetail(
            data.ImposesDisadvantageOnTriggeringAttackRoll,
            data.GrantsAdvantageOnNextAttackRollIfMissed,
            data.AdvantageDurationTrigger,
            data.RecoversOnShortRest);
    }
}
