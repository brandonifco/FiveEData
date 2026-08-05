namespace FiveEData.Rules.Creatures.Conditions;

internal static class ConditionDefinitionValidator
{
    public static IReadOnlyList<string> Validate(
        ConditionDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(definition.Id.Value))
        {
            errors.Add("Condition ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(definition.Name))
        {
            errors.Add("Condition name must not be empty.");
        }

        if (definition.Sources.Count == 0)
        {
            errors.Add(
                "Condition must have at least one source reference.");
        }

        return errors;
    }

    public static void EnsureValid(
        ConditionDefinition definition)
    {
        IReadOnlyList<string> errors = Validate(definition);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Condition definition '{definition.Id}' is invalid:" +
            Environment.NewLine +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
