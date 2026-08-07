namespace FiveEData.Rules.Spells.MagicSchools;

internal static class MagicSchoolDefinitionValidator
{
    public static IReadOnlyList<string> Validate(
        MagicSchoolDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(definition.Id.Value))
        {
            errors.Add("Magic school ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(definition.Name))
        {
            errors.Add("Magic school name must not be empty.");
        }

        if (definition.Sources.Count == 0)
        {
            errors.Add(
                "Magic school must have at least one source reference.");
        }

        return errors;
    }

    public static void EnsureValid(
        MagicSchoolDefinition definition)
    {
        IReadOnlyList<string> errors = Validate(definition);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Magic school definition '{definition.Id}' is invalid:" +
            Environment.NewLine +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
