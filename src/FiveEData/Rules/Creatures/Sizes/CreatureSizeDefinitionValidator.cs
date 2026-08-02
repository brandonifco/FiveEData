namespace FiveEData.Rules.Creatures.Sizes;

internal static class CreatureSizeDefinitionValidator
{
    public static IReadOnlyList<string> Validate(
        CreatureSizeDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(definition.Id.Value))
        {
            errors.Add("Creature-size ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(definition.Name))
        {
            errors.Add("Creature-size name must not be empty.");
        }

        if (definition.Sources.Count == 0)
        {
            errors.Add(
                "Creature size must have at least one source reference.");
        }

        return errors;
    }

    public static void EnsureValid(
        CreatureSizeDefinition definition)
    {
        IReadOnlyList<string> errors = Validate(definition);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Creature-size definition '{definition.Id}' is invalid:" +
            Environment.NewLine +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
