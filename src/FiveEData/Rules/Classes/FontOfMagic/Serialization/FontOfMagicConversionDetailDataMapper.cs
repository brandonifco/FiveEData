namespace FiveEData.Rules.Classes.FontOfMagic.Serialization;

internal static class FontOfMagicConversionDetailDataMapper
{
    public static FontOfMagicConversionDetail Map(
        FontOfMagicConversionDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        FontOfMagicSlotCostGrantData[] slotCostData =
            data.SlotCostByLevel
            ?? throw new ArgumentException(
                "Font of Magic conversion slot cost by level is required.",
                nameof(data));

        FontOfMagicSlotCostGrant[] slotCostByLevel = slotCostData
            .Select(
                grant => new FontOfMagicSlotCostGrant(
                    grant.SpellSlotLevel,
                    grant.SorceryPointCost))
            .ToArray();

        return new FontOfMagicConversionDetail(slotCostByLevel);
    }
}
