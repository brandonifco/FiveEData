using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;

namespace FiveEData.Rules.Adventuring.DowntimeActivities.Serialization;

internal sealed class DowntimeActivityDefinitionData
{
    [JsonRequired]
    public string? Id { get; init; }

    [JsonRequired]
    public string? Name { get; init; }

    [JsonRequired]
    public int? RequiredDays { get; init; }

    [JsonRequired]
    public int? CostPerDayGoldPieces { get; init; }

    [JsonRequired]
    public string? SavingThrowAbilityId { get; init; }

    [JsonRequired]
    public int? SavingThrowDC { get; init; }

    [JsonRequired]
    public int? MarketValueProgressPerDayGoldPieces { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}
