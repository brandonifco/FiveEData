using FiveEData.Rules.Common;

namespace FiveEData.Rules.Expenses.Services;

internal static class MundaneServiceDefinitionValidator
{
    public static IReadOnlyList<string> Validate(
        MundaneServiceDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(definition.Id.Value))
        {
            errors.Add("Mundane-service ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(definition.Name))
        {
            errors.Add("Mundane-service name must not be empty.");
        }

        if (definition.Cost.Amount.CopperPieces <= 0)
        {
            errors.Add(
                "Mundane-service cost must be greater than zero.");
        }

        if (!Enum.IsDefined(definition.Cost.Kind))
        {
            errors.Add(
                "Mundane-service cost kind must be defined.");
        }

        if (!Enum.IsDefined(definition.PricingUnit))
        {
            errors.Add(
                "Mundane-service pricing unit must be defined.");
        }

        var ruleIds = new HashSet<RuleId>();

        foreach (RuleId ruleId in definition.SpecialRuleIds)
        {
            if (string.IsNullOrWhiteSpace(ruleId.Value))
            {
                errors.Add(
                    "Mundane-service special rule ID must not be empty.");
                continue;
            }

            if (!ruleIds.Add(ruleId))
            {
                errors.Add(
                    "Mundane-service special rule ID " +
                    $"'{ruleId}' is duplicated.");
            }
        }

        if (definition.Sources.Count == 0)
        {
            errors.Add(
                "Mundane service must have at least one source reference.");
        }

        return errors;
    }

    public static void EnsureValid(
        MundaneServiceDefinition definition)
    {
        IReadOnlyList<string> errors = Validate(definition);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Mundane-service definition '{definition.Id}' is invalid:" +
            Environment.NewLine +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
