using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;

namespace FiveEData.Rules.Adventuring.Resting.Serialization;

internal sealed class RestTypeDefinitionData
{
    [JsonRequired]
    public string? Id { get; init; }

    [JsonRequired]
    public string? Name { get; init; }

    [JsonRequired]
    public int MinimumDurationHours { get; init; }

    [JsonRequired]
    public int? CooldownHours { get; init; }

    [JsonRequired]
    public int? MinimumHitPointsToBenefit { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}
