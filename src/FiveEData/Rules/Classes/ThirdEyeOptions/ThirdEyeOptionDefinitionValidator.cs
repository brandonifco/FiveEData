namespace FiveEData.Rules.Classes.ThirdEyeOptions;

internal static class ThirdEyeOptionDefinitionValidator
{
    public static IReadOnlyList<string> Validate(
        ThirdEyeOptionDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(definition.Id.Value))
        {
            errors.Add("Third Eye option ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(definition.Name))
        {
            errors.Add("Third Eye option name must not be empty.");
        }

        ValidatePositive(
            definition.DarkvisionRangeFeet,
            "darkvision range",
            errors);

        ValidatePositive(
            definition.EtherealSightRangeFeet,
            "ethereal sight range",
            errors);

        ValidatePositive(
            definition.SeeInvisibilityRangeFeet,
            "see invisibility range",
            errors);

        if (definition.Sources.Count == 0)
        {
            errors.Add(
                "Third Eye option must have at least one source reference.");
        }

        return errors;
    }

    public static void EnsureValid(ThirdEyeOptionDefinition definition)
    {
        IReadOnlyList<string> errors = Validate(definition);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Third Eye option definition '{definition.Id}' is " +
            $"invalid:{Environment.NewLine}" +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }

    private static void ValidatePositive(
        int? value,
        string description,
        List<string> errors)
    {
        if (value is { } amount && amount <= 0)
        {
            errors.Add(
                $"Third Eye option {description} must be greater than zero.");
        }
    }
}
