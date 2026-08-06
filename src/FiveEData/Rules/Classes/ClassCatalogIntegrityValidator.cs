using FiveEData.Rules.Catalog;
using FiveEData.Rules.Classes.ExtraAttack;
using FiveEData.Rules.Classes.Spellcasting;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Skills;
using FiveEData.Rules.Equipment.Weapons;

namespace FiveEData.Rules.Classes;

internal static class ClassCatalogIntegrityValidator
{
    public static IReadOnlyList<string> Validate(
        ClassDefinitionSet definitions,
        IReadOnlySet<SourceDocumentId> sourceIds,
        IReadOnlySet<AbilityId> abilityIds,
        IReadOnlySet<SkillId> skillIds,
        IReadOnlySet<WeaponId> weaponIds,
        IReadOnlySet<RuleId> ruleIds,
        IReadOnlySet<SpellSlotProgressionId> spellSlotProgressionIds,
        IReadOnlySet<ExtraAttackProgressionId> extraAttackProgressionIds)
    {
        ArgumentNullException.ThrowIfNull(definitions);
        ArgumentNullException.ThrowIfNull(sourceIds);
        ArgumentNullException.ThrowIfNull(abilityIds);
        ArgumentNullException.ThrowIfNull(skillIds);
        ArgumentNullException.ThrowIfNull(weaponIds);
        ArgumentNullException.ThrowIfNull(ruleIds);
        ArgumentNullException.ThrowIfNull(spellSlotProgressionIds);
        ArgumentNullException.ThrowIfNull(extraAttackProgressionIds);

        var errors = new List<string>();

        HashSet<ClassId> classIds =
            definitions.Classes
                .Select(definition => definition.Id)
                .ToHashSet();

        foreach (
            ClassDefinition @class
            in definitions.Classes
                .OrderBy(item => item.Id.Value, StringComparer.Ordinal))
        {
            string owner = $"Class '{@class.Id}'";

            ValidateSources(owner, @class.Sources, sourceIds, errors);

            foreach (AbilityId abilityId in @class.PrimaryAbilityIds)
            {
                if (!abilityIds.Contains(abilityId))
                {
                    errors.Add(
                        $"{owner} references missing primary ability " +
                        $"'{abilityId}'.");
                }
            }

            foreach (AbilityId abilityId in @class.SavingThrowProficiencyIds)
            {
                if (!abilityIds.Contains(abilityId))
                {
                    errors.Add(
                        $"{owner} references missing saving throw " +
                        $"ability '{abilityId}'.");
                }
            }

            foreach (WeaponId weaponId in @class.WeaponProficiencyIds)
            {
                if (!weaponIds.Contains(weaponId))
                {
                    errors.Add(
                        $"{owner} references missing weapon " +
                        $"'{weaponId}'.");
                }
            }

            foreach (SkillId skillId in @class.SkillChoiceOptionIds)
            {
                if (!skillIds.Contains(skillId))
                {
                    errors.Add(
                        $"{owner} references missing skill choice " +
                        $"option '{skillId}'.");
                }
            }

            foreach (ClassLevelFeature feature in @class.LevelFeatures)
            {
                if (!ruleIds.Contains(feature.FeatureRuleId))
                {
                    errors.Add(
                        $"{owner} references missing level feature rule " +
                        $"'{feature.FeatureRuleId}'.");
                }
            }

            ValidateSpellcasting(
                owner,
                @class.SpellSlotProgressionId,
                @class.SpellcastingAbilityId,
                spellSlotProgressionIds,
                abilityIds,
                errors);

            if (@class.ExtraAttackProgressionId is { } extraAttackId &&
                !extraAttackProgressionIds.Contains(extraAttackId))
            {
                errors.Add(
                    $"{owner} references missing Extra Attack " +
                    $"progression '{extraAttackId}'.");
            }
        }

        foreach (
            SubclassDefinition subclass
            in definitions.Subclasses
                .OrderBy(item => item.Id.Value, StringComparer.Ordinal))
        {
            string owner = $"Subclass '{subclass.Id}'";

            ValidateSources(owner, subclass.Sources, sourceIds, errors);

            if (!classIds.Contains(subclass.ClassId))
            {
                errors.Add(
                    $"{owner} references missing class '{subclass.ClassId}'.");
            }

            foreach (ClassLevelFeature feature in subclass.LevelFeatures)
            {
                if (!ruleIds.Contains(feature.FeatureRuleId))
                {
                    errors.Add(
                        $"{owner} references missing level feature rule " +
                        $"'{feature.FeatureRuleId}'.");
                }
            }

            ValidateSpellcasting(
                owner,
                subclass.SpellSlotProgressionId,
                subclass.SpellcastingAbilityId,
                spellSlotProgressionIds,
                abilityIds,
                errors);
        }

        return errors;
    }

    private static void ValidateSpellcasting(
        string owner,
        SpellSlotProgressionId? spellSlotProgressionId,
        AbilityId? spellcastingAbilityId,
        IReadOnlySet<SpellSlotProgressionId> spellSlotProgressionIds,
        IReadOnlySet<AbilityId> abilityIds,
        ICollection<string> errors)
    {
        if (spellSlotProgressionId is { } progressionId &&
            !spellSlotProgressionIds.Contains(progressionId))
        {
            errors.Add(
                $"{owner} references missing spell slot progression " +
                $"'{progressionId}'.");
        }

        if (spellcastingAbilityId is { } abilityId &&
            !abilityIds.Contains(abilityId))
        {
            errors.Add(
                $"{owner} references missing spellcasting ability " +
                $"'{abilityId}'.");
        }

        if (spellSlotProgressionId is null != spellcastingAbilityId is null)
        {
            errors.Add(
                $"{owner} must define both a spell slot progression " +
                "and a spellcasting ability, or neither.");
        }
    }

    private static void ValidateSources(
        string owner,
        IReadOnlyList<SourceReference> sources,
        IReadOnlySet<SourceDocumentId> sourceIds,
        ICollection<string> errors)
    {
        foreach (SourceReference source in sources)
        {
            if (!sourceIds.Contains(source.DocumentId))
            {
                errors.Add(
                    $"{owner} references missing source document " +
                    $"'{source.DocumentId}'.");
            }
        }
    }
}
