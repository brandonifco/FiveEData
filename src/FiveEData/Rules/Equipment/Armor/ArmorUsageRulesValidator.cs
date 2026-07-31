using FiveEData.Rules.Common;

namespace FiveEData.Rules.Equipment.Armor;

internal static class ArmorUsageRulesValidator
{
    public static void EnsureValid(ArmorUsageRules rules)
    {
        ArgumentNullException.ThrowIfNull(rules);

        EnsureRuleId(rules.ArmorProficiencyRuleId, nameof(rules.ArmorProficiencyRuleId));
        EnsureRuleId(rules.StrengthSpeedRuleId, nameof(rules.StrengthSpeedRuleId));
        EnsureRuleId(rules.StealthRuleId, nameof(rules.StealthRuleId));
        EnsureRuleId(rules.ShieldRuleId, nameof(rules.ShieldRuleId));
        EnsureRuleId(rules.DonDoffRuleId, nameof(rules.DonDoffRuleId));

        if (rules.InsufficientStrengthSpeedReduction.Feet <= 0)
        {
            throw new InvalidOperationException(
                "Insufficient-Strength speed reduction must be greater than zero feet.");
        }

        if (rules.ShieldHandsRequired <= 0)
        {
            throw new InvalidOperationException(
                "Shield hands required must be greater than zero.");
        }

        if (rules.MaximumBenefitingShields <= 0)
        {
            throw new InvalidOperationException(
                "Maximum benefiting shields must be greater than zero.");
        }

        if (rules.DoffingWithHelpDivisor <= 1)
        {
            throw new InvalidOperationException(
                "Doffing-with-help divisor must be greater than one.");
        }

        ValidateTiming(
            rules.LightArmorChangeTiming,
            nameof(rules.LightArmorChangeTiming));
        ValidateTiming(
            rules.MediumArmorChangeTiming,
            nameof(rules.MediumArmorChangeTiming));
        ValidateTiming(
            rules.HeavyArmorChangeTiming,
            nameof(rules.HeavyArmorChangeTiming));
        ValidateTiming(
            rules.ShieldChangeTiming,
            nameof(rules.ShieldChangeTiming));

        if (rules.Sources.Count == 0)
        {
            throw new InvalidOperationException(
                "Armor usage rules must have at least one source reference.");
        }
    }

    private static void ValidateTiming(
        EquipmentChangeTiming timing,
        string propertyName)
    {
        if (timing.Don.Amount <= 0 || timing.Doff.Amount <= 0)
        {
            throw new InvalidOperationException(
                $"Armor usage timing '{propertyName}' must have positive don and doff durations.");
        }

        if (!Enum.IsDefined(timing.Don.Unit) ||
            !Enum.IsDefined(timing.Doff.Unit))
        {
            throw new InvalidOperationException(
                $"Armor usage timing '{propertyName}' contains an undefined time unit.");
        }
    }

    private static void EnsureRuleId(RuleId id, string propertyName)
    {
        if (string.IsNullOrWhiteSpace(id.Value))
        {
            throw new InvalidOperationException(
                $"Armor usage rule reference '{propertyName}' must not be empty.");
        }
    }
}
