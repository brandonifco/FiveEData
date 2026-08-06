namespace FiveEData.Rules.Classes.ElementalDisciplines;

internal static class ElementalDisciplineDefinitionValidator
{
    public static IReadOnlyList<string> Validate(
        ElementalDisciplineDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(definition.Id.Value))
        {
            errors.Add("Elemental discipline ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(definition.Name))
        {
            errors.Add("Elemental discipline name must not be empty.");
        }

        if (definition.KiPointCost is { } kiPointCost && kiPointCost <= 0)
        {
            errors.Add(
                "Elemental discipline ki point cost must be greater than " +
                "zero.");
        }

        if (definition.RequiredMinimumLevel is { } requiredMinimumLevel &&
            requiredMinimumLevel is < 1 or > 20)
        {
            errors.Add(
                "Elemental discipline required minimum level must be " +
                "between 1 and 20.");
        }

        if (definition.Sources.Count == 0)
        {
            errors.Add(
                "Elemental discipline must have at least one source " +
                "reference.");
        }

        return errors;
    }

    public static void EnsureValid(ElementalDisciplineDefinition definition)
    {
        IReadOnlyList<string> errors = Validate(definition);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Elemental discipline definition '{definition.Id}' is " +
            $"invalid:{Environment.NewLine}" +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
