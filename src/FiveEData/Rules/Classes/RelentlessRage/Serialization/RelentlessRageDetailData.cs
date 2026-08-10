using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.RelentlessRage.Serialization;

internal sealed class RelentlessRageDetailData
{
    [JsonRequired]
    public string? SavingThrowAbilityId { get; init; }

    [JsonRequired]
    public int InitialSavingThrowDC { get; init; }

    [JsonRequired]
    public int SavingThrowDCIncreasePerUse { get; init; }

    [JsonRequired]
    public int HitPointsRetained { get; init; }

    [JsonRequired]
    public bool ResetsOnShortRest { get; init; }
}
