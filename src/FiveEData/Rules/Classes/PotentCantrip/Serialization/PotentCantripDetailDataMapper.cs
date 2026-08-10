namespace FiveEData.Rules.Classes.PotentCantrip.Serialization;

internal static class PotentCantripDetailDataMapper
{
    public static PotentCantripDetail Map(PotentCantripDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return new PotentCantripDetail(
            data.GrantsHalfDamageOnSuccessfulSave,
            data.NegatesAdditionalCantripEffectsOnSuccessfulSave);
    }
}
