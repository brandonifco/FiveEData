namespace FiveEData.Rules.Combat.CombatActions;

internal static class CombatActionDefinitionValidator
{
    public static IReadOnlyList<string> Validate(
        CombatActionDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(definition.Id.Value))
        {
            errors.Add("Combat action ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(definition.Name))
        {
            errors.Add("Combat action name must not be empty.");
        }

        if (definition.Sources.Count == 0)
        {
            errors.Add(
                "Combat action must have at least one source reference.");
        }

        return errors;
    }

    public static void EnsureValid(
        CombatActionDefinition definition)
    {
        IReadOnlyList<string> errors = Validate(definition);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Combat action definition '{definition.Id}' is invalid:" +
            Environment.NewLine +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
