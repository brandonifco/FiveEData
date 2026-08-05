using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;

namespace FiveEData.Rules.Creatures.Races.Serialization;

internal sealed class RaceDefinitionData
{
    [JsonRequired]
    public string? Id { get; init; }

    [JsonRequired]
    public string? Name { get; init; }

    [JsonRequired]
    public string? Size { get; init; }

    [JsonRequired]
    public int SpeedFeet { get; init; }

    [JsonRequired]
    public RaceAbilityScoreIncreaseData[]? AbilityScoreIncreases { get; init; }

    [JsonRequired]
    public int ChoosableAbilityScoreIncreaseCount { get; init; }

    [JsonRequired]
    public string[]? LanguageIds { get; init; }

    [JsonRequired]
    public int AdditionalLanguageChoiceCount { get; init; }

    [JsonRequired]
    public string[]? TraitRuleIds { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}

internal sealed class RaceAbilityScoreIncreaseData
{
    [JsonRequired]
    public string? AbilityId { get; init; }

    [JsonRequired]
    public int Bonus { get; init; }
}
