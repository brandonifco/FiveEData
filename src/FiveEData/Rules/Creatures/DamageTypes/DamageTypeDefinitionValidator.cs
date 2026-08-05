namespace FiveEData.Rules.Creatures.DamageTypes;

internal static class DamageTypeDefinitionValidator
{
    public static IReadOnlyList<string> Validate(
        DamageTypeDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(definition.Id.Value))
        {
            errors.Add("Damage type ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(definition.Name))
        {
            errors.Add("Damage type name must not be empty.");
        }

        if (definition.Sources.Count == 0)
        {
            errors.Add(
                "Damage type must have at least one source reference.");
        }

        return errors;
    }

    public static void EnsureValid(
        DamageTypeDefinition definition)
    {
        IReadOnlyList<string> errors = Validate(definition);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Damage type definition '{definition.Id}' is invalid:" +
            Environment.NewLine +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
