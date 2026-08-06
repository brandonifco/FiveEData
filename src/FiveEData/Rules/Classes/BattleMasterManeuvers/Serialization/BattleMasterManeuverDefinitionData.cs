using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;

namespace FiveEData.Rules.Classes.BattleMasterManeuvers.Serialization;

internal sealed class BattleMasterManeuverDefinitionData
{
    [JsonRequired]
    public string? Id { get; init; }

    [JsonRequired]
    public string? Name { get; init; }

    [JsonRequired]
    public BattleMasterManeuverEffectTarget EffectTarget { get; init; }

    [JsonRequired]
    public string? SavingThrowAbilityId { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}
