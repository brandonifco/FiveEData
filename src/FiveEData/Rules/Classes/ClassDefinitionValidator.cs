using FiveEData.Rules.Common;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Skills;
using FiveEData.Rules.Equipment.Armor;
using FiveEData.Rules.Equipment.Weapons;

namespace FiveEData.Rules.Classes;

internal static class ClassDefinitionValidator
{
    public static IReadOnlyList<string> Validate(ClassDefinition @class)
    {
        ArgumentNullException.ThrowIfNull(@class);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(@class.Id.Value))
        {
            errors.Add("Class ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(@class.Name))
        {
            errors.Add("Class name must not be empty.");
        }

        if (@class.Sources.Count == 0)
        {
            errors.Add("Class must have at least one source reference.");
        }

        if (@class.PrimaryAbilityIds.Count == 0)
        {
            errors.Add("Class must declare at least one primary ability.");
        }

        ValidateDistinct(
            @class.PrimaryAbilityIds,
            "primary ability",
            errors);

        if (@class.SavingThrowProficiencyIds.Count != 2)
        {
            errors.Add(
                "Class must declare exactly two saving throw proficiencies.");
        }

        ValidateDistinct(
            @class.SavingThrowProficiencyIds,
            "saving throw proficiency",
            errors);

        ValidateDistinctDefinedEnum(
            @class.ArmorProficiencyCategories,
            "armor proficiency category",
            errors);

        ValidateDistinctDefinedEnum(
            @class.WeaponProficiencyCategories,
            "weapon proficiency category",
            errors);

        ValidateDistinct(
            @class.WeaponProficiencyIds,
            "weapon proficiency",
            errors);

        if (@class.SkillChoiceCount < 0)
        {
            errors.Add("Class skill choice count cannot be negative.");
        }

        if (@class.SkillChoiceCount > @class.SkillChoiceOptionIds.Count)
        {
            errors.Add(
                "Class skill choice count cannot exceed the number of " +
                "skill choice options.");
        }

        ValidateDistinct(
            @class.SkillChoiceOptionIds,
            "skill choice option",
            errors);

        var seenLevelFeatures = new HashSet<(int Level, RuleId FeatureRuleId)>();

        foreach (ClassLevelFeature feature in @class.LevelFeatures)
        {
            if (!seenLevelFeatures.Add((feature.Level, feature.FeatureRuleId)))
            {
                errors.Add(
                    $"Class level feature '{feature.FeatureRuleId}' is " +
                    $"duplicated at level {feature.Level}.");
            }
        }

        return errors;
    }

    public static void EnsureValid(ClassDefinition @class)
    {
        IReadOnlyList<string> errors = Validate(@class);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Class definition '{@class.Id}' is invalid:{Environment.NewLine}" +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }

    private static void ValidateDistinct(
        IReadOnlyList<AbilityId> ids,
        string label,
        ICollection<string> errors)
    {
        var seen = new HashSet<AbilityId>();

        foreach (AbilityId id in ids)
        {
            if (string.IsNullOrWhiteSpace(id.Value))
            {
                errors.Add($"Class {label} ID must not be empty.");
                continue;
            }

            if (!seen.Add(id))
            {
                errors.Add($"Class {label} '{id}' is duplicated.");
            }
        }
    }

    private static void ValidateDistinct(
        IReadOnlyList<WeaponId> ids,
        string label,
        ICollection<string> errors)
    {
        var seen = new HashSet<WeaponId>();

        foreach (WeaponId id in ids)
        {
            if (string.IsNullOrWhiteSpace(id.Value))
            {
                errors.Add($"Class {label} ID must not be empty.");
                continue;
            }

            if (!seen.Add(id))
            {
                errors.Add($"Class {label} '{id}' is duplicated.");
            }
        }
    }

    private static void ValidateDistinct(
        IReadOnlyList<SkillId> ids,
        string label,
        ICollection<string> errors)
    {
        var seen = new HashSet<SkillId>();

        foreach (SkillId id in ids)
        {
            if (string.IsNullOrWhiteSpace(id.Value))
            {
                errors.Add($"Class {label} ID must not be empty.");
                continue;
            }

            if (!seen.Add(id))
            {
                errors.Add($"Class {label} '{id}' is duplicated.");
            }
        }
    }

    private static void ValidateDistinctDefinedEnum(
        IReadOnlyList<ArmorCategory> values,
        string label,
        ICollection<string> errors)
    {
        var seen = new HashSet<ArmorCategory>();

        foreach (ArmorCategory value in values)
        {
            if (!Enum.IsDefined(value))
            {
                errors.Add($"Class {label} value '{value}' must be defined.");
                continue;
            }

            if (!seen.Add(value))
            {
                errors.Add($"Class {label} '{value}' is duplicated.");
            }
        }
    }

    private static void ValidateDistinctDefinedEnum(
        IReadOnlyList<WeaponProficiencyCategory> values,
        string label,
        ICollection<string> errors)
    {
        var seen = new HashSet<WeaponProficiencyCategory>();

        foreach (WeaponProficiencyCategory value in values)
        {
            if (!Enum.IsDefined(value))
            {
                errors.Add($"Class {label} value '{value}' must be defined.");
                continue;
            }

            if (!seen.Add(value))
            {
                errors.Add($"Class {label} '{value}' is duplicated.");
            }
        }
    }
}
