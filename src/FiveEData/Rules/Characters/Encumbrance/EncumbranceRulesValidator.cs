namespace FiveEData.Rules.Characters.Encumbrance;

internal static class EncumbranceRulesValidator
{
    public static IReadOnlyList<string> Validate(EncumbranceRules rules)
    {
        ArgumentNullException.ThrowIfNull(rules);

        var errors = new List<string>();

        if (rules.SizeCarryingCapacityMultipliers.Count == 0)
        {
            errors.Add(
                "Encumbrance must have at least one size carrying " +
                "capacity multiplier.");
        }

        if (rules.SizeCarryingCapacityMultipliers
                .Select(grant => grant.SizeId)
                .Distinct()
                .Count() !=
            rules.SizeCarryingCapacityMultipliers.Count)
        {
            errors.Add(
                "Encumbrance must not repeat a creature size in its " +
                "carrying capacity multipliers.");
        }

        if (rules.EncumberedCarryingCapacityMultiplier <= 0)
        {
            errors.Add(
                "Encumbered carrying capacity multiplier must be " +
                "positive.");
        }

        if (rules.EncumberedSpeedReductionFeet <= 0)
        {
            errors.Add("Encumbered speed reduction must be positive.");
        }

        if (rules.HeavilyEncumberedCarryingCapacityMultiplier <=
            rules.EncumberedCarryingCapacityMultiplier)
        {
            errors.Add(
                "Heavily encumbered carrying capacity multiplier must " +
                "exceed the encumbered multiplier.");
        }

        if (rules.HeavilyEncumberedSpeedReductionFeet <=
            rules.EncumberedSpeedReductionFeet)
        {
            errors.Add(
                "Heavily encumbered speed reduction must exceed the " +
                "encumbered speed reduction.");
        }

        if (rules.HeavilyEncumberedDisadvantageAbilityIds.Count == 0)
        {
            errors.Add(
                "Heavily encumbered must name at least one ability that " +
                "has disadvantage.");
        }

        if (rules.HeavilyEncumberedDisadvantageAbilityIds
                .Distinct()
                .Count() !=
            rules.HeavilyEncumberedDisadvantageAbilityIds.Count)
        {
            errors.Add(
                "Heavily encumbered must not repeat an ability that has " +
                "disadvantage.");
        }

        if (rules.Sources.Count == 0)
        {
            errors.Add("Encumbrance must have at least one source.");
        }

        return errors;
    }

    public static void EnsureValid(EncumbranceRules rules)
    {
        IReadOnlyList<string> errors = Validate(rules);

        if (errors.Count > 0)
        {
            throw new InvalidOperationException(
                "Invalid encumbrance rules: " + string.Join(" ", errors));
        }
    }
}
