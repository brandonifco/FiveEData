using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Skills;

namespace FiveEData.Rules.Backgrounds;

internal static class BackgroundCatalogIntegrityValidator
{
    public static IReadOnlyList<string> Validate(
        IReadOnlyList<BackgroundDefinition> backgrounds,
        IReadOnlySet<SourceDocumentId> sourceIds,
        IReadOnlySet<SkillId> skillIds,
        IReadOnlySet<RuleId> ruleIds)
    {
        ArgumentNullException.ThrowIfNull(backgrounds);
        ArgumentNullException.ThrowIfNull(sourceIds);
        ArgumentNullException.ThrowIfNull(skillIds);
        ArgumentNullException.ThrowIfNull(ruleIds);

        var errors = new List<string>();

        foreach (
            BackgroundDefinition background
            in backgrounds
                .OrderBy(item => item.Id.Value, StringComparer.Ordinal))
        {
            string owner = $"Background '{background.Id}'";

            ValidateSources(owner, background.Sources, sourceIds, errors);

            foreach (SkillId skillId in background.SkillProficiencyIds)
            {
                if (!skillIds.Contains(skillId))
                {
                    errors.Add(
                        $"{owner} references missing skill '{skillId}'.");
                }
            }

            if (!ruleIds.Contains(background.FeatureRuleId))
            {
                errors.Add(
                    $"{owner} references missing feature rule " +
                    $"'{background.FeatureRuleId}'.");
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
