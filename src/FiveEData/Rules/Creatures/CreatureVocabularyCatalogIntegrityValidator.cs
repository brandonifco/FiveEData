using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Skills;

namespace FiveEData.Rules.Creatures;

internal static class CreatureVocabularyCatalogIntegrityValidator
{
    public static IReadOnlyList<string> Validate(
        CreatureVocabularyDefinitionSet definitions,
        IReadOnlySet<SourceDocumentId> sourceIds)
    {
        ArgumentNullException.ThrowIfNull(definitions);
        ArgumentNullException.ThrowIfNull(sourceIds);

        var errors = new List<string>();

        HashSet<AbilityId> abilityIds =
            definitions.Abilities
                .Select(definition => definition.Id)
                .ToHashSet();

        foreach (
            AbilityDefinition definition
            in definitions.Abilities
                .OrderBy(
                    item => item.Id.Value,
                    StringComparer.Ordinal))
        {
            ValidateSources(
                $"Ability '{definition.Id}'",
                definition.Sources,
                sourceIds,
                errors);
        }

        foreach (
            SkillDefinition definition
            in definitions.Skills
                .OrderBy(
                    item => item.Id.Value,
                    StringComparer.Ordinal))
        {
            ValidateSources(
                $"Skill '{definition.Id}'",
                definition.Sources,
                sourceIds,
                errors);

            if (!abilityIds.Contains(
                    definition.NormallyAssociatedAbilityId))
            {
                errors.Add(
                    $"Skill '{definition.Id}' references missing " +
                    "normally associated ability " +
                    $"'{definition.NormallyAssociatedAbilityId}'.");
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
        foreach (
            SourceReference source
            in sources
                .OrderBy(
                    item => item.DocumentId.Value,
                    StringComparer.Ordinal)
                .ThenBy(item => item.Page)
                .ThenBy(
                    item => item.Section,
                    StringComparer.Ordinal))
        {
            if (!sourceIds.Contains(source.DocumentId))
            {
                errors.Add(
                    $"{owner} references missing source " +
                    $"document '{source.DocumentId}'.");
            }
        }
    }
}
