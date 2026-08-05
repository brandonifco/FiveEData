namespace FiveEData.Rules.Creatures.Senses;

internal static class SenseDefinitionValidator
{
    public static IReadOnlyList<string> Validate(
        SenseDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(definition.Id.Value))
        {
            errors.Add("Sense ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(definition.Name))
        {
            errors.Add("Sense name must not be empty.");
        }

        if (definition.Sources.Count == 0)
        {
            errors.Add(
                "Sense must have at least one source reference.");
        }

        return errors;
    }

    public static void EnsureValid(
        SenseDefinition definition)
    {
        IReadOnlyList<string> errors = Validate(definition);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Sense definition '{definition.Id}' is invalid:" +
            Environment.NewLine +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
