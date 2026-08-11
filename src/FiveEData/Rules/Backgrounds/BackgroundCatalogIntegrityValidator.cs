using FiveEData.Rules.Equipment.Tools;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Skills;
using FiveEData.Rules.Expenses.Lifestyles;

namespace FiveEData.Rules.Backgrounds;

internal static class BackgroundCatalogIntegrityValidator
{
    public static IReadOnlyList<string> Validate(
        IReadOnlyList<BackgroundDefinition> backgrounds,
        IReadOnlySet<SourceDocumentId> sourceIds,
        IReadOnlySet<SkillId> skillIds,
        IReadOnlySet<RuleId> ruleIds,
        IReadOnlySet<LifestyleId> lifestyleIds,
        IReadOnlySet<ToolId> toolIds,
        IReadOnlySet<ToolFamilyId> toolFamilyIds)
    {
        ArgumentNullException.ThrowIfNull(backgrounds);
        ArgumentNullException.ThrowIfNull(sourceIds);
        ArgumentNullException.ThrowIfNull(skillIds);
        ArgumentNullException.ThrowIfNull(ruleIds);
        ArgumentNullException.ThrowIfNull(lifestyleIds);
        ArgumentNullException.ThrowIfNull(toolIds);
        ArgumentNullException.ThrowIfNull(toolFamilyIds);

        var errors = new List<string>();

        foreach (
            BackgroundDefinition background
            in backgrounds
                .OrderBy(item => item.Id.Value, StringComparer.Ordinal))
        {
            string owner = $"Background '{background.Id}'";

            foreach (ToolId toolId in background.ToolProficiencyIds)
            {
                if (!toolIds.Contains(toolId))
                {
                    errors.Add(
                        $"{owner} references missing tool '{toolId}'.");
                }
            }

            if (background.ToolProficiencyChoice is { } toolChoice)
            {
                foreach (ToolFamilyId familyId in toolChoice.ToolFamilyIds)
                {
                    if (!toolFamilyIds.Contains(familyId))
                    {
                        errors.Add(
                            $"{owner} references missing tool family " +
                            $"'{familyId}'.");
                    }
                }

                foreach (ToolId toolId in toolChoice.ToolOptionIds)
                {
                    if (!toolIds.Contains(toolId))
                    {
                        errors.Add(
                            $"{owner} references missing tool '{toolId}'.");
                    }
                }
            }

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

            if (background.SustainedLifestyleId is { } sustainedLifestyleId &&
                !lifestyleIds.Contains(sustainedLifestyleId))
            {
                errors.Add(
                    $"{owner} references missing lifestyle " +
                    $"'{sustainedLifestyleId}'.");
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
