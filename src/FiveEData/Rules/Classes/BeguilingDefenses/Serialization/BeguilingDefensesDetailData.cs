using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.BeguilingDefenses.Serialization;

internal sealed class BeguilingDefensesDetailData
{
    [JsonRequired]
    public string? ImmuneConditionId { get; init; }

    [JsonRequired]
    public string? ReflectionSavingThrowAbilityId { get; init; }

    [JsonRequired]
    public int ReflectionDurationMinutes { get; init; }
}
