using FiveEData.Rules.Common;

namespace FiveEData.Rules.Expenses.Lifestyles;

internal static class LifestyleDefinitionValidator
{
    public static IReadOnlyList<string> Validate(
        LifestyleDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(definition.Id.Value))
        {
            errors.Add("Lifestyle ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(definition.Name))
        {
            errors.Add("Lifestyle name must not be empty.");
        }

        if (definition.DailyCost is { } dailyCost)
        {
            if (dailyCost.Amount.CopperPieces <= 0)
            {
                errors.Add(
                    "Lifestyle daily cost must be greater than zero when specified.");
            }

            if (!Enum.IsDefined(dailyCost.Kind))
            {
                errors.Add(
                    "Lifestyle daily cost kind must be defined when specified.");
            }
        }

        var ruleIds = new HashSet<RuleId>();

        foreach (RuleId ruleId in definition.SpecialRuleIds)
        {
            if (string.IsNullOrWhiteSpace(ruleId.Value))
            {
                errors.Add(
                    "Lifestyle special rule ID must not be empty.");
                continue;
            }

            if (!ruleIds.Add(ruleId))
            {
                errors.Add(
                    $"Lifestyle special rule ID '{ruleId}' is duplicated.");
            }
        }

        if (definition.Sources.Count == 0)
        {
            errors.Add(
                "Lifestyle must have at least one source reference.");
        }

        return errors;
    }

    public static void EnsureValid(LifestyleDefinition definition)
    {
        IReadOnlyList<string> errors = Validate(definition);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Lifestyle definition '{definition.Id}' is invalid:" +
            Environment.NewLine +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
