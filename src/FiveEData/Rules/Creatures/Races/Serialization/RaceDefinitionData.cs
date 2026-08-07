using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Creatures.Races.BreathWeapon.Serialization;
using FiveEData.Rules.Creatures.Races.Lucky.Serialization;
using FiveEData.Rules.Creatures.Races.RelentlessEndurance.Serialization;
using FiveEData.Rules.Creatures.Races.SavageAttacks.Serialization;

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
    public int? DarkvisionRangeFeet { get; init; }

    [JsonRequired]
    public string[]? ResistedDamageTypeIds { get; init; }

    [JsonRequired]
    public int? TranceDurationHours { get; init; }

    [JsonRequired]
    public BreathWeaponProgressionDetailData? BreathWeaponProgression
    {
        get;
        init;
    }

    [JsonRequired]
    public SavageAttacksDetailData? SavageAttacks { get; init; }

    [JsonRequired]
    public RelentlessEnduranceDetailData? RelentlessEndurance
    {
        get;
        init;
    }

    [JsonRequired]
    public LuckyDetailData? Lucky { get; init; }

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
