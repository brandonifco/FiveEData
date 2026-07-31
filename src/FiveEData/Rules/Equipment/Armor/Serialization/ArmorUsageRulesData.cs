using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Equipment;

namespace FiveEData.Rules.Equipment.Armor.Serialization;

internal sealed class ArmorUsageRulesData
{
    [JsonRequired]
    public string? ArmorProficiencyRuleId { get; init; }

    [JsonRequired]
    public string? StrengthSpeedRuleId { get; init; }

    [JsonRequired]
    public string? StealthRuleId { get; init; }

    [JsonRequired]
    public string? ShieldRuleId { get; init; }

    [JsonRequired]
    public string? DonDoffRuleId { get; init; }

    [JsonRequired]
    public ArmorProficiencyConsequencesData? ArmorProficiencyConsequences { get; init; }

    [JsonRequired]
    public int InsufficientStrengthSpeedReductionFeet { get; init; }

    [JsonRequired]
    public int ShieldHandsRequired { get; init; }

    [JsonRequired]
    public int MaximumBenefitingShields { get; init; }

    [JsonRequired]
    public bool RequiresFullDonDurationForArmorClassBenefit { get; init; }

    [JsonRequired]
    public int DoffingWithHelpDivisor { get; init; }

    [JsonRequired]
    public EquipmentChangeTimingData? LightArmorChangeTiming { get; init; }

    [JsonRequired]
    public EquipmentChangeTimingData? MediumArmorChangeTiming { get; init; }

    [JsonRequired]
    public EquipmentChangeTimingData? HeavyArmorChangeTiming { get; init; }

    [JsonRequired]
    public EquipmentChangeTimingData? ShieldChangeTiming { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}

internal sealed class ArmorProficiencyConsequencesData
{
    [JsonRequired]
    public bool DisadvantageOnStrengthOrDexterityAbilityChecks { get; init; }

    [JsonRequired]
    public bool DisadvantageOnStrengthOrDexteritySavingThrows { get; init; }

    [JsonRequired]
    public bool DisadvantageOnStrengthOrDexterityAttackRolls { get; init; }

    [JsonRequired]
    public bool PreventsSpellcasting { get; init; }
}

internal sealed class EquipmentChangeTimingData
{
    [JsonRequired]
    public EquipmentChangeDurationData? Don { get; init; }

    [JsonRequired]
    public EquipmentChangeDurationData? Doff { get; init; }
}

internal sealed class EquipmentChangeDurationData
{
    [JsonRequired]
    public int Amount { get; init; }

    [JsonRequired]
    public EquipmentChangeTimeUnit Unit { get; init; }
}
