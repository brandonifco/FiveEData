namespace FiveEData.Rules.Spells.Concentration;

internal static class ConcentrationRulesValidator
{
    public static IReadOnlyList<string> Validate(ConcentrationRules rules)
    {
        ArgumentNullException.ThrowIfNull(rules);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(rules.SavingThrowAbilityId.Value))
        {
            errors.Add("Concentration saving throw ability ID is required.");
        }

        if (rules.MinimumSavingThrowDC < 1)
        {
            errors.Add(
                "Concentration minimum saving throw DC must be positive.");
        }

        if (rules.DamageDivisorForSavingThrowDC < 1)
        {
            errors.Add(
                "Concentration damage divisor for the saving throw DC must " +
                "be positive.");
        }

        if (rules.EndedByConditionIds.Count == 0)
        {
            errors.Add(
                "Concentration must name at least one condition that ends " +
                "it.");
        }

        if (rules.EndedByConditionIds.Distinct().Count() !=
            rules.EndedByConditionIds.Count)
        {
            errors.Add(
                "Concentration must not repeat a condition that ends it.");
        }

        if (rules.Sources.Count == 0)
        {
            errors.Add("Concentration must have at least one source.");
        }

        return errors;
    }

    public static void EnsureValid(ConcentrationRules rules)
    {
        IReadOnlyList<string> errors = Validate(rules);

        if (errors.Count > 0)
        {
            throw new InvalidOperationException(
                "Invalid concentration rules: " + string.Join(" ", errors));
        }
    }
}
