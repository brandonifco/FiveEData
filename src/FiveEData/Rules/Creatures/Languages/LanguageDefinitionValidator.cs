namespace FiveEData.Rules.Creatures.Languages;

internal static class LanguageDefinitionValidator
{
    public static IReadOnlyList<string> Validate(
        LanguageDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(definition.Id.Value))
        {
            errors.Add("Language ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(definition.Name))
        {
            errors.Add("Language name must not be empty.");
        }

        if (!Enum.IsDefined(definition.Category))
        {
            errors.Add("Language category must be defined.");
        }

        if (definition.Sources.Count == 0)
        {
            errors.Add(
                "Language must have at least one source reference.");
        }

        return errors;
    }

    public static void EnsureValid(
        LanguageDefinition definition)
    {
        IReadOnlyList<string> errors = Validate(definition);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Language definition '{definition.Id}' is invalid:" +
            Environment.NewLine +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
