using FiveEData.Rules.Common;

namespace FiveEData.Rules.Expenses.FoodAndLodging;

internal static class FoodDrinkDefinitionValidator
{
    public static IReadOnlyList<string> Validate(
        FoodDrinkDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(definition.Id.Value))
        {
            errors.Add("Food-and-drink ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(definition.Name))
        {
            errors.Add("Food-and-drink name must not be empty.");
        }

        if (definition.Cost.CopperPieces <= 0)
        {
            errors.Add(
                "Food-and-drink cost must be greater than zero.");
        }

        if (!Enum.IsDefined(definition.PricingUnit))
        {
            errors.Add(
                "Food-and-drink pricing unit must be defined.");
        }

        var ruleIds = new HashSet<RuleId>();

        foreach (RuleId ruleId in definition.SpecialRuleIds)
        {
            if (string.IsNullOrWhiteSpace(ruleId.Value))
            {
                errors.Add(
                    "Food-and-drink special rule ID must not be empty.");
                continue;
            }

            if (!ruleIds.Add(ruleId))
            {
                errors.Add(
                    $"Food-and-drink special rule ID '{ruleId}' is duplicated.");
            }
        }

        if (definition.Sources.Count == 0)
        {
            errors.Add(
                "Food-and-drink definition must have at least one source reference.");
        }

        return errors;
    }

    public static void EnsureValid(
        FoodDrinkDefinition definition)
    {
        IReadOnlyList<string> errors = Validate(definition);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Food-and-drink definition '{definition.Id}' is invalid:" +
            Environment.NewLine +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
