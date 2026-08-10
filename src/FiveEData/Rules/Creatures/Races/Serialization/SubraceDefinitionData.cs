using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Equipment.Armor;

namespace FiveEData.Rules.Creatures.Races.Serialization;

internal sealed class SubraceDefinitionData
{
    [JsonRequired]
    public string? Id { get; init; }

    [JsonRequired]
    public string? Name { get; init; }

    [JsonRequired]
    public string? RaceId { get; init; }

    [JsonRequired]
    public RaceAbilityScoreIncreaseData[]? AbilityScoreIncreases { get; init; }

    [JsonRequired]
    public int? SpeedFeet { get; init; }

    [JsonRequired]
    public int AdditionalLanguageChoiceCount { get; init; }

    [JsonRequired]
    public string[]? TraitRuleIds { get; init; }

    [JsonRequired]
    public int? DarkvisionRangeFeet { get; init; }

    [JsonRequired]
    public string[]? ResistedDamageTypeIds { get; init; }

    [JsonRequired]
    public int? HitPointBonusPerLevel { get; init; }

    [JsonRequired]
    public string[]? WeaponProficiencyIds { get; init; }

    [JsonRequired]
    public ArmorCategory[]? ArmorProficiencyCategories { get; init; }

    [JsonRequired]
    public SpellGrantData[]? InnateSpellGrants { get; init; }

    [JsonRequired]
    public string? InnateSpellcastingAbilityId { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}
