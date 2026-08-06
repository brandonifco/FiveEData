namespace FiveEData.Rules.Classes.EldritchInvocations;

internal static class EldritchInvocationDefinitionValidator
{
    public static IReadOnlyList<string> Validate(
        EldritchInvocationDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(definition.Id.Value))
        {
            errors.Add("Eldritch invocation ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(definition.Name))
        {
            errors.Add("Eldritch invocation name must not be empty.");
        }

        if (definition.RequiredMinimumLevel is { } requiredMinimumLevel &&
            requiredMinimumLevel is < 1 or > 20)
        {
            errors.Add(
                "Eldritch invocation required minimum level must be " +
                "between 1 and 20.");
        }

        if (definition.RequiresPactBoon is { } requiresPactBoon &&
            !Enum.IsDefined(requiresPactBoon))
        {
            errors.Add(
                "Eldritch invocation required Pact Boon must be a " +
                "defined value.");
        }

        if (definition.Sources.Count == 0)
        {
            errors.Add(
                "Eldritch invocation must have at least one source " +
                "reference.");
        }

        return errors;
    }

    public static void EnsureValid(EldritchInvocationDefinition definition)
    {
        IReadOnlyList<string> errors = Validate(definition);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Eldritch invocation definition '{definition.Id}' is " +
            $"invalid:{Environment.NewLine}" +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
