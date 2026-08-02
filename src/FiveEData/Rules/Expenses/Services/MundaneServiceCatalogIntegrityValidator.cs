using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Expenses.Services;

internal static class MundaneServiceCatalogIntegrityValidator
{
    private static readonly IReadOnlyDictionary<
        MundaneServiceId,
        IReadOnlyList<RuleId>> ExpectedAssociations =
            new Dictionary<
                MundaneServiceId,
                IReadOnlyList<RuleId>>
            {
                [
                    new MundaneServiceId(
                        "dnd5e2014.mundane-service." +
                        "coach-between-towns")
                ] = [],
                [
                    new MundaneServiceId(
                        "dnd5e2014.mundane-service." +
                        "coach-within-city")
                ] = [],
                [
                    new MundaneServiceId(
                        "dnd5e2014.mundane-service." +
                        "hireling-skilled")
                ] =
                [
                    new RuleId(
                        "dnd5e2014.expense-rule." +
                        "skilled-hireling-proficiency-service"),
                    new RuleId(
                        "dnd5e2014.expense-rule." +
                        "skilled-hireling-pay-minimum")
                ],
                [
                    new MundaneServiceId(
                        "dnd5e2014.mundane-service." +
                        "hireling-untrained")
                ] =
                [
                    new RuleId(
                        "dnd5e2014.expense-rule." +
                        "untrained-hireling-menial-work")
                ],
                [
                    new MundaneServiceId(
                        "dnd5e2014.mundane-service.messenger")
                ] = [],
                [
                    new MundaneServiceId(
                        "dnd5e2014.mundane-service." +
                        "road-or-gate-toll")
                ] = [],
                [
                    new MundaneServiceId(
                        "dnd5e2014.mundane-service." +
                        "ship-passage")
                ] = []
            };

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

            if (ExpectedAssociations.TryGetValue(
                    definition.Id,
                    out IReadOnlyList<RuleId>? expectedRuleIds))
            {
                ValidateExactRuleAssociations(
                    identity,
                    definition.SpecialRuleIds,
                    expectedRuleIds,
                    errors);
            }
        }

        return errors;
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

        foreach (
            RuleId missing
            in expected
                .Except(actual)
                .OrderBy(
                    ruleId => ruleId.Value,
                    StringComparer.Ordinal))
        {
            errors.Add(
                $"{owner} is missing required rule " +
                $"association '{missing}'.");
        }

        foreach (
            RuleId forbidden
            in actual
                .Except(expected)
                .OrderBy(
                    ruleId => ruleId.Value,
                    StringComparer.Ordinal))
        {
            errors.Add(
                $"{owner} has forbidden rule association " +
                $"'{forbidden}'.");
        }
    }
}
