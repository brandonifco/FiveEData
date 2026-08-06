using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.FontOfMagic.Serialization;

internal sealed class FontOfMagicConversionDetailData
{
    [JsonRequired]
    public FontOfMagicSlotCostGrantData[]? SlotCostByLevel { get; init; }
}

internal sealed class FontOfMagicSlotCostGrantData
{
    [JsonRequired]
    public int SpellSlotLevel { get; init; }

    [JsonRequired]
    public int SorceryPointCost { get; init; }
}
