namespace FiveEData.Rules.Creatures.Alignments;

internal static class AlignmentDefinitionValidator
{
    public static IReadOnlyList<string> Validate(
        AlignmentDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(definition.Id.Value))
        {
            errors.Add("Alignment ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(definition.Name))
        {
            errors.Add("Alignment name must not be empty.");
        }

        if (!Enum.IsDefined(definition.Ethic))
        {
            errors.Add("Alignment ethic must be defined.");
        }

        if (!Enum.IsDefined(definition.Morality))
        {
            errors.Add("Alignment morality must be defined.");
        }

        if (definition.Sources.Count == 0)
        {
            errors.Add(
                "Alignment must have at least one source reference.");
        }

        return errors;
    }

    public static void EnsureValid(
        AlignmentDefinition definition)
    {
        IReadOnlyList<string> errors = Validate(definition);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Alignment definition '{definition.Id}' is invalid:" +
            Environment.NewLine +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
