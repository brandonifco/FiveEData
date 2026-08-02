namespace FiveEData.Rules.Creatures.Abilities;

internal static class AbilityDefinitionValidator
{
    public static IReadOnlyList<string> Validate(
        AbilityDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(definition.Id.Value))
        {
            errors.Add("Ability ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(definition.Name))
        {
            errors.Add("Ability name must not be empty.");
        }

        if (definition.Sources.Count == 0)
        {
            errors.Add(
                "Ability must have at least one source reference.");
        }

        return errors;
    }

    public static void EnsureValid(AbilityDefinition definition)
    {
        IReadOnlyList<string> errors = Validate(definition);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Ability definition '{definition.Id}' is invalid:" +
            Environment.NewLine +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
