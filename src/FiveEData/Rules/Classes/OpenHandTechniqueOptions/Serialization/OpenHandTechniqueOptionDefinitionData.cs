using System.Text.Json.Serialization;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance.Serialization;

namespace FiveEData.Rules.Classes.OpenHandTechniqueOptions.Serialization;

internal sealed class OpenHandTechniqueOptionDefinitionData
{
    [JsonRequired]
    public string? Id { get; init; }

    [JsonRequired]
    public string? Name { get; init; }

    [JsonRequired]
    public string? SavingThrowAbilityId { get; init; }

    [JsonRequired]
    public string? ImposedConditionId { get; init; }

    [JsonRequired]
    public int? PushDistanceFeet { get; init; }

    [JsonRequired]
    public bool PreventsReactions { get; init; }

    [JsonRequired]
    public NextTurnDurationTrigger? PreventsReactionsUntil { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}
