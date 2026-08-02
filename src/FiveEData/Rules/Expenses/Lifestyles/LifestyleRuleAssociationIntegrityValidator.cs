using FiveEData.Rules.Common;

namespace FiveEData.Rules.Expenses.Lifestyles;

internal static class
    LifestyleRuleAssociationIntegrityValidator
{
    private static readonly IReadOnlyDictionary<
        LifestyleId,
        RuleId> ExpectedAssociations =
            new Dictionary<LifestyleId, RuleId>
            {
                [
                    new LifestyleId(
                        "dnd5e2014.lifestyle.wretched")
                ] =
                    new RuleId(
                        "dnd5e2014.lifestyle-rule." +
                        "wretched-conditions"),
                [
                    new LifestyleId(
                        "dnd5e2014.lifestyle.squalid")
                ] =
                    new RuleId(
                        "dnd5e2014.lifestyle-rule." +
                        "squalid-conditions"),
                [
                    new LifestyleId(
                        "dnd5e2014.lifestyle.poor")
                ] =
                    new RuleId(
                        "dnd5e2014.lifestyle-rule." +
                        "poor-conditions"),
                [
                    new LifestyleId(
                        "dnd5e2014.lifestyle.modest")
                ] =
                    new RuleId(
                        "dnd5e2014.lifestyle-rule." +
                        "modest-conditions"),
                [
                    new LifestyleId(
                        "dnd5e2014.lifestyle.comfortable")
                ] =
                    new RuleId(
                        "dnd5e2014.lifestyle-rule." +
                        "comfortable-conditions"),
                [
                    new LifestyleId(
                        "dnd5e2014.lifestyle.wealthy")
                ] =
                    new RuleId(
                        "dnd5e2014.lifestyle-rule." +
                        "wealthy-conditions"),
                [
                    new LifestyleId(
                        "dnd5e2014.lifestyle.aristocratic")
                ] =
                    new RuleId(
                        "dnd5e2014.lifestyle-rule." +
                        "aristocratic-conditions")
            };

    public static IReadOnlyList<string> Validate(
        IReadOnlyList<LifestyleDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        var errors = new List<string>();

        foreach (LifestyleDefinition definition in definitions)
        {
            if (!ExpectedAssociations.TryGetValue(
                    definition.Id,
                    out RuleId expectedRuleId))
            {
                continue;
            }

            HashSet<RuleId> actualRuleIds =
                definition.SpecialRuleIds.ToHashSet();

            if (!actualRuleIds.Contains(expectedRuleId))
            {
                errors.Add(
                    $"Lifestyle '{definition.Id}' is missing " +
                    "required rule association " +
                    $"'{expectedRuleId}'.");
            }

            foreach (
                RuleId forbiddenRuleId
                in actualRuleIds
                    .Where(
                        ruleId =>
                            ruleId != expectedRuleId)
                    .OrderBy(
                        ruleId => ruleId.Value,
                        StringComparer.Ordinal))
            {
                errors.Add(
                    $"Lifestyle '{definition.Id}' has forbidden " +
                    "rule association " +
                    $"'{forbiddenRuleId}'.");
            }
        }

        return errors;
    }
}
