using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Equipment.Armor;
using FiveEData.Rules.Equipment.Weapons;

namespace FiveEData.Rules.Classes.Serialization;

internal sealed class ClassDefinitionData
{
    [JsonRequired]
    public string? Id { get; init; }

    [JsonRequired]
    public string? Name { get; init; }

    [JsonRequired]
    public int HitDieSides { get; init; }

    [JsonRequired]
    public string[]? PrimaryAbilityIds { get; init; }

    [JsonRequired]
    public bool RequiresAllPrimaryAbilities { get; init; }

    [JsonRequired]
    public string[]? SavingThrowProficiencyIds { get; init; }

    [JsonRequired]
    public ArmorCategory[]? ArmorProficiencyCategories { get; init; }

    [JsonRequired]
    public bool ProficientWithShields { get; init; }

    [JsonRequired]
    public WeaponProficiencyCategory[]? WeaponProficiencyCategories { get; init; }

    [JsonRequired]
    public string[]? WeaponProficiencyIds { get; init; }

    [JsonRequired]
    public int SkillChoiceCount { get; init; }

    [JsonRequired]
    public string[]? SkillChoiceOptionIds { get; init; }

    [JsonRequired]
    public ClassLevelFeatureData[]? LevelFeatures { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}

internal sealed class ClassLevelFeatureData
{
    [JsonRequired]
    public int Level { get; init; }

    [JsonRequired]
    public string? FeatureRuleId { get; init; }
}
