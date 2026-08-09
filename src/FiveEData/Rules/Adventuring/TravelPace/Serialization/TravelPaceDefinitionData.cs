using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;

namespace FiveEData.Rules.Adventuring.TravelPace.Serialization;

internal sealed class TravelPaceDefinitionData
{
    [JsonRequired]
    public string? Id { get; init; }

    [JsonRequired]
    public string? Name { get; init; }

    [JsonRequired]
    public int FeetPerMinute { get; init; }

    [JsonRequired]
    public int MilesPerHour { get; init; }

    [JsonRequired]
    public int MilesPerDay { get; init; }

    [JsonRequired]
    public int? PassiveWisdomPerceptionPenalty { get; init; }

    [JsonRequired]
    public bool AllowsStealth { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}
