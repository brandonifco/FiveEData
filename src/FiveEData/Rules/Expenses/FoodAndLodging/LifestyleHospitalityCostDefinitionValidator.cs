using FiveEData.Rules.Common;

namespace FiveEData.Rules.Expenses.FoodAndLodging;

internal static class LifestyleHospitalityCostDefinitionValidator
{
    public static IReadOnlyList<string> Validate(
        LifestyleHospitalityCostDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(
                definition.LifestyleId.Value))
        {
            errors.Add(
                "Hospitality-cost lifestyle ID must not be empty.");
        }

        if (definition.InnStayCostPerDay.CopperPieces <= 0)
        {
            errors.Add(
                "Hospitality inn-stay cost per day must be greater than zero.");
        }

        if (definition.MealsCostPerDay.CopperPieces <= 0)
        {
            errors.Add(
                "Hospitality meals cost per day must be greater than zero.");
        }

        var ruleIds = new HashSet<RuleId>();

        foreach (RuleId ruleId in definition.SpecialRuleIds)
        {
            if (string.IsNullOrWhiteSpace(ruleId.Value))
            {
                errors.Add(
                    "Hospitality special rule ID must not be empty.");
                continue;
            }

            if (!ruleIds.Add(ruleId))
            {
                errors.Add(
                    $"Hospitality special rule ID '{ruleId}' is duplicated.");
            }
        }

        if (definition.Sources.Count == 0)
        {
            errors.Add(
                "Hospitality cost must have at least one source reference.");
        }

        return errors;
    }

    public static void EnsureValid(
        LifestyleHospitalityCostDefinition definition)
    {
        IReadOnlyList<string> errors = Validate(definition);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            "Hospitality cost for lifestyle " +
            $"'{definition.LifestyleId}' is invalid:" +
            Environment.NewLine +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
