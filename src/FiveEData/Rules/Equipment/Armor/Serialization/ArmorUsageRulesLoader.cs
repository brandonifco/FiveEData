using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Equipment;

namespace FiveEData.Rules.Equipment.Armor.Serialization;

internal static class ArmorUsageRulesLoader
{
    public static ArmorUsageRules LoadFromFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static ArmorUsageRules LoadFromJson(string json)
    {
        ArmorUsageRulesData data =
            StrictJson.DeserializeObject<ArmorUsageRulesData>(
                json,
                "Armor-usage rules");

        try
        {
            ArmorUsageRules rules = Map(data);
            ArmorUsageRulesValidator.EnsureValid(rules);
            return rules;
        }
        catch (Exception exception)
            when (exception is ArgumentException or InvalidOperationException)
        {
            throw new InvalidDataException(
                "Invalid armor-usage rules definition.",
                exception);
        }
    }

    private static ArmorUsageRules Map(ArmorUsageRulesData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        ArmorProficiencyConsequencesData proficiency =
            data.ArmorProficiencyConsequences
            ?? throw new ArgumentException(
                "Armor-proficiency consequences are required.",
                nameof(data));

        SourceReferenceData[] sourceData = data.Sources
            ?? throw new ArgumentException(
                "Armor-usage rule sources are required.",
                nameof(data));

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new ArmorUsageRules(
            CreateRuleId(data.ArmorProficiencyRuleId, "armor-proficiency"),
            CreateRuleId(data.StrengthSpeedRuleId, "Strength-speed"),
            CreateRuleId(data.StealthRuleId, "Stealth"),
            CreateRuleId(data.ShieldRuleId, "shield"),
            CreateRuleId(data.DonDoffRuleId, "don/doff"),
            new ArmorProficiencyConsequences(
                proficiency.DisadvantageOnStrengthOrDexterityAbilityChecks,
                proficiency.DisadvantageOnStrengthOrDexteritySavingThrows,
                proficiency.DisadvantageOnStrengthOrDexterityAttackRolls,
                proficiency.PreventsSpellcasting),
            new Distance(data.InsufficientStrengthSpeedReductionFeet),
            data.ShieldHandsRequired,
            data.MaximumBenefitingShields,
            data.RequiresFullDonDurationForArmorClassBenefit,
            data.DoffingWithHelpDivisor,
            MapTiming(data.LightArmorChangeTiming, "light-armor"),
            MapTiming(data.MediumArmorChangeTiming, "medium-armor"),
            MapTiming(data.HeavyArmorChangeTiming, "heavy-armor"),
            MapTiming(data.ShieldChangeTiming, "shield"),
            sources);
    }

    private static RuleId CreateRuleId(string? value, string description)
    {
        return new RuleId(
            value
            ?? throw new ArgumentException(
                $"Armor-usage {description} rule ID is required."));
    }

    private static EquipmentChangeTiming MapTiming(
        EquipmentChangeTimingData? data,
        string description)
    {
        if (data is null)
        {
            throw new ArgumentException(
                $"Armor-usage {description} change timing is required.");
        }

        return new EquipmentChangeTiming(
            MapDuration(data.Don, $"{description} don"),
            MapDuration(data.Doff, $"{description} doff"));
    }

    private static EquipmentChangeDuration MapDuration(
        EquipmentChangeDurationData? data,
        string description)
    {
        if (data is null)
        {
            throw new ArgumentException(
                $"Armor-usage {description} duration is required.");
        }

        return new EquipmentChangeDuration(
            data.Amount,
            data.Unit);
    }
}
