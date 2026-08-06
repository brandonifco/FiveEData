namespace FiveEData.Rules.Classes.FontOfMagic;

public sealed record FontOfMagicConversionDetail
{
    public FontOfMagicConversionDetail(
        IEnumerable<FontOfMagicSlotCostGrant> slotCostByLevel)
    {
        ArgumentNullException.ThrowIfNull(slotCostByLevel);

        SlotCostByLevel = Array.AsReadOnly(slotCostByLevel.ToArray());
    }

    public IReadOnlyList<FontOfMagicSlotCostGrant> SlotCostByLevel { get; }
}
