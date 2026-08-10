using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;

namespace FiveEData.Rules.Classes.ThirdEyeOptions.Serialization;

internal sealed class ThirdEyeOptionDefinitionData
{
    [JsonRequired]
    public string? Id { get; init; }

    [JsonRequired]
    public string? Name { get; init; }

    [JsonRequired]
    public int? DarkvisionRangeFeet { get; init; }

    [JsonRequired]
    public int? EtherealSightRangeFeet { get; init; }

    [JsonRequired]
    public int? SeeInvisibilityRangeFeet { get; init; }

    [JsonRequired]
    public bool CanReadAllLanguages { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}
