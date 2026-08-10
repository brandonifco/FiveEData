namespace FiveEData.Rules.Classes.ElementalAffinity.Serialization;

internal static class ElementalAffinityDetailDataMapper
{
    public static ElementalAffinityDetail Map(
        ElementalAffinityDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return new ElementalAffinityDetail(
            data.AddsSpellcastingModifierToDamage,
            data.ResistanceSorceryPointCost,
            data.ResistanceDurationHours);
    }
}
