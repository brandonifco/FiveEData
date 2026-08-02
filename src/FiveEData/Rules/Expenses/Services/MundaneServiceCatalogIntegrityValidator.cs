using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Expenses.Services;

internal static class MundaneServiceCatalogIntegrityValidator
{
    private static readonly MundaneServiceId SkilledHirelingId =
        new(
            "dnd5e2014.mundane-service." +
            "hireling-skilled");

    private static readonly MundaneServiceId
        UntrainedHirelingId =
            new(
                "dnd5e2014.mundane-service." +
                "hireling-untrained");

    private static readonly RuleId
        SkilledHirelingProficiencyRuleId =
            new(
                "dnd5e2014.expense-rule." +
                "skilled-hireling-proficiency-service");

    private static readonly RuleId
        SkilledHirelingMinimumPayRuleId =
            new(
                "dnd5e2014.expense-rule." +
                "skilled-hireling-pay-minimum");

    private static readonly RuleId
        UntrainedHirelingMenialWorkRuleId =
            new(
                "dnd5e2014.expense-rule." +
                "untrained-hireling-menial-work");

    public static IReadOnlyList<string> Validate(
        IReadOnlyList<MundaneServiceDefinition> definitions,
        IReadOnlySet<SourceDocumentId> sourceIds,
        IReadOnlySet<RuleId> ruleIds)
    {
        ArgumentNullException.ThrowIfNull(definitions);
        ArgumentNullException.ThrowIfNull(sourceIds);
        ArgumentNullException.ThrowIfNull(ruleIds);

        var errors = new List<string>();

        foreach (MundaneServiceDefinition definition in definitions)
        {
            string identity =
                $"Mundane service '{definition.Id}'";

            foreach (SourceReference source in definition.Sources)
            {
                if (!sourceIds.Contains(source.DocumentId))
                {
                    errors.Add(
                        $"{identity} references missing source " +
                        $"document '{source.DocumentId}'.");
                }
            }

            foreach (RuleId ruleId in definition.SpecialRuleIds)
            {
                if (!ruleIds.Contains(ruleId))
                {
                    errors.Add(
                        $"{identity} references missing rule " +
                        $"'{ruleId}'.");
                }
            }

            ValidateExactRuleAssociations(
                identity,
                definition.SpecialRuleIds,
                GetExpectedRuleIds(definition.Id),
                errors);
        }

        return errors;
    }

    private static IReadOnlyList<RuleId> GetExpectedRuleIds(
        MundaneServiceId id)
    {
        if (id == SkilledHirelingId)
        {
            return
            [
                SkilledHirelingProficiencyRuleId,
                SkilledHirelingMinimumPayRuleId
            ];
        }

        if (id == UntrainedHirelingId)
        {
            return [UntrainedHirelingMenialWorkRuleId];
        }

        return [];
    }

    private static void ValidateExactRuleAssociations(
        string owner,
        IReadOnlyList<RuleId> actualRuleIds,
        IReadOnlyList<RuleId> expectedRuleIds,
        ICollection<string> errors)
    {
        HashSet<RuleId> actual =
            actualRuleIds.ToHashSet();

        HashSet<RuleId> expected =
            expectedRuleIds.ToHashSet();

        foreach (RuleId missing in expected.Except(actual))
        {
            errors.Add(
                $"{owner} is missing required rule " +
                $"association '{missing}'.");
        }

        foreach (RuleId forbidden in actual.Except(expected))
        {
            errors.Add(
                $"{owner} has forbidden rule association " +
                $"'{forbidden}'.");
        }
    }
}
