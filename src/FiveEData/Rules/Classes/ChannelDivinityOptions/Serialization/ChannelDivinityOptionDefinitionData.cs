using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;

namespace FiveEData.Rules.Classes.ChannelDivinityOptions.Serialization;

internal sealed class ChannelDivinityOptionDefinitionData
{
    [JsonRequired]
    public string? Id { get; init; }

    [JsonRequired]
    public string? Name { get; init; }

    [JsonRequired]
    public int? RangeFeet { get; init; }

    [JsonRequired]
    public string? SavingThrowAbilityId { get; init; }

    [JsonRequired]
    public int? DurationMinutes { get; init; }

    [JsonRequired]
    public int? RollBonus { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}
