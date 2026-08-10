using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.DamageTypes;
using FiveEData.Rules.Creatures.Languages;
using FiveEData.Rules.Creatures.Sizes;
using FiveEData.Rules.Creatures.Skills;
using FiveEData.Rules.Equipment.Weapons;

namespace FiveEData.Rules.Creatures.Races;

internal static class RaceCatalogIntegrityValidator
{
    public static IReadOnlyList<string> Validate(
        RaceDefinitionSet definitions,
        IReadOnlySet<SourceDocumentId> sourceIds,
        IReadOnlySet<AbilityId> abilityIds,
        IReadOnlySet<CreatureSizeId> sizeIds,
        IReadOnlySet<LanguageId> languageIds,
        IReadOnlySet<RuleId> ruleIds,
        IReadOnlySet<DamageTypeId> damageTypeIds,
        IReadOnlySet<WeaponId> weaponIds,
        IReadOnlySet<SkillId> skillIds)
    {
        ArgumentNullException.ThrowIfNull(definitions);
        ArgumentNullException.ThrowIfNull(sourceIds);
        ArgumentNullException.ThrowIfNull(abilityIds);
        ArgumentNullException.ThrowIfNull(sizeIds);
        ArgumentNullException.ThrowIfNull(languageIds);
        ArgumentNullException.ThrowIfNull(ruleIds);
        ArgumentNullException.ThrowIfNull(damageTypeIds);
        ArgumentNullException.ThrowIfNull(weaponIds);
        ArgumentNullException.ThrowIfNull(skillIds);

        var errors = new List<string>();

        HashSet<RaceId> raceIds =
            definitions.Races
                .Select(definition => definition.Id)
                .ToHashSet();

        foreach (
            RaceDefinition race
            in definitions.Races
                .OrderBy(item => item.Id.Value, StringComparer.Ordinal))
        {
            string owner = $"Race '{race.Id}'";

            ValidateSources(owner, race.Sources, sourceIds, errors);

            if (!sizeIds.Contains(race.Size))
            {
                errors.Add(
                    $"{owner} references missing size '{race.Size}'.");
            }

            foreach (RaceAbilityScoreIncrease increase in race.AbilityScoreIncreases)
            {
                if (!abilityIds.Contains(increase.AbilityId))
                {
                    errors.Add(
                        $"{owner} references missing ability " +
                        $"'{increase.AbilityId}'.");
                }
            }

            foreach (LanguageId languageId in race.LanguageIds)
            {
                if (!languageIds.Contains(languageId))
                {
                    errors.Add(
                        $"{owner} references missing language " +
                        $"'{languageId}'.");
                }
            }

            foreach (RuleId traitRuleId in race.TraitRuleIds)
            {
                if (!ruleIds.Contains(traitRuleId))
                {
                    errors.Add(
                        $"{owner} references missing trait rule " +
                        $"'{traitRuleId}'.");
                }
            }

            foreach (DamageTypeId damageTypeId in race.ResistedDamageTypeIds)
            {
                if (!damageTypeIds.Contains(damageTypeId))
                {
                    errors.Add(
                        $"{owner} references missing damage type " +
                        $"'{damageTypeId}'.");
                }
            }

            foreach (WeaponId weaponId in race.WeaponProficiencyIds)
            {
                if (!weaponIds.Contains(weaponId))
                {
                    errors.Add(
                        $"{owner} references missing weapon " +
                        $"'{weaponId}'.");
                }
            }

            if (race.SkillProficiencyId is { } skillProficiencyId &&
                !skillIds.Contains(skillProficiencyId))
            {
                errors.Add(
                    $"{owner} references missing skill " +
                    $"'{skillProficiencyId}'.");
            }
        }

        foreach (
            SubraceDefinition subrace
            in definitions.Subraces
                .OrderBy(item => item.Id.Value, StringComparer.Ordinal))
        {
            string owner = $"Subrace '{subrace.Id}'";

            ValidateSources(owner, subrace.Sources, sourceIds, errors);

            if (!raceIds.Contains(subrace.RaceId))
            {
                errors.Add(
                    $"{owner} references missing race '{subrace.RaceId}'.");
            }

            foreach (RaceAbilityScoreIncrease increase in subrace.AbilityScoreIncreases)
            {
                if (!abilityIds.Contains(increase.AbilityId))
                {
                    errors.Add(
                        $"{owner} references missing ability " +
                        $"'{increase.AbilityId}'.");
                }
            }

            foreach (RuleId traitRuleId in subrace.TraitRuleIds)
            {
                if (!ruleIds.Contains(traitRuleId))
                {
                    errors.Add(
                        $"{owner} references missing trait rule " +
                        $"'{traitRuleId}'.");
                }
            }

            foreach (
                DamageTypeId damageTypeId in subrace.ResistedDamageTypeIds)
            {
                if (!damageTypeIds.Contains(damageTypeId))
                {
                    errors.Add(
                        $"{owner} references missing damage type " +
                        $"'{damageTypeId}'.");
                }
            }

            foreach (WeaponId weaponId in subrace.WeaponProficiencyIds)
            {
                if (!weaponIds.Contains(weaponId))
                {
                    errors.Add(
                        $"{owner} references missing weapon " +
                        $"'{weaponId}'.");
                }
            }
        }

        return errors;
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
