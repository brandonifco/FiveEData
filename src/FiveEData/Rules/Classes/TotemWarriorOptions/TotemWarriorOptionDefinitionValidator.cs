namespace FiveEData.Rules.Classes.TotemWarriorOptions;

internal static class TotemWarriorOptionDefinitionValidator
{
    public static IReadOnlyList<string> Validate(
        TotemWarriorOptionDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(definition.Id.Value))
        {
            errors.Add("Totem warrior option ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(definition.Name))
        {
            errors.Add("Totem warrior option name must not be empty.");
        }

        if (definition.RequiredLevel is < 1 or > 20)
        {
            errors.Add(
                "Totem warrior option required level must be between 1 " +
                "and 20.");
        }

        ValidatePositiveDistance(
            definition.GrantsAlliesAdvantageOnMeleeAttacksWithinFeet,
            "ally melee attack advantage range",
            errors);

        ValidatePositiveDistance(
            definition.ClearSightRangeFeet,
            "clear sight range",
            errors);

        ValidatePositiveDistance(
            definition.ClearSightDetailEquivalentRangeFeet,
            "clear sight detail equivalent range",
            errors);

        ValidatePositiveDistance(
            definition.ImposesDisadvantageOnAttacksAgainstOthersWithinFeet,
            "attack disadvantage range",
            errors);

        bool hasClearSightRange = definition.ClearSightRangeFeet is not null;
        bool hasClearSightDetailRange =
            definition.ClearSightDetailEquivalentRangeFeet is not null;

        if (hasClearSightRange != hasClearSightDetailRange)
        {
            errors.Add(
                "Totem warrior option must have a clear sight range and a " +
                "clear sight detail equivalent range together, or neither.");
        }

        bool hasImposedCondition = definition.ImposedConditionId is not null;

        if (definition.MaximumTargetSizeId is not null && !hasImposedCondition)
        {
            errors.Add(
                "Totem warrior option cannot have a maximum target size " +
                "without an imposed condition.");
        }

        if (definition.ImposedConditionRequiresBonusAction &&
            !hasImposedCondition)
        {
            errors.Add(
                "Totem warrior option cannot require a bonus action to " +
                "impose a condition without an imposed condition.");
        }

        if (definition.Sources.Count == 0)
        {
            errors.Add(
                "Totem warrior option must have at least one source " +
                "reference.");
        }

        return errors;
    }

    public static void EnsureValid(TotemWarriorOptionDefinition definition)
    {
        IReadOnlyList<string> errors = Validate(definition);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Totem warrior option definition '{definition.Id}' is " +
            $"invalid:{Environment.NewLine}" +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }

    private static void ValidatePositiveDistance(
        int? value,
        string description,
        List<string> errors)
    {
        if (value is { } distance && distance <= 0)
        {
            errors.Add(
                $"Totem warrior option {description} must be greater than " +
                $"zero.");
        }
    }
}
