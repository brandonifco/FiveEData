namespace FiveEData.Rules.Adventuring.Resting;

internal static class RestTypeDefinitionValidator
{
    public static IReadOnlyList<string> Validate(
        RestTypeDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(definition.Id.Value))
        {
            errors.Add("Rest type ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(definition.Name))
        {
            errors.Add("Rest type name must not be empty.");
        }

        if (definition.MinimumDurationHours <= 0)
        {
            errors.Add(
                "Rest type minimum duration must be greater than zero " +
                "hours.");
        }

        if (definition.CooldownHours is { } cooldownHours &&
            cooldownHours <= 0)
        {
            errors.Add(
                "Rest type cooldown must be greater than zero hours.");
        }

        if (definition.MinimumHitPointsToBenefit is
                { } minimumHitPointsToBenefit &&
            minimumHitPointsToBenefit <= 0)
        {
            errors.Add(
                "Rest type minimum hit points to benefit must be " +
                "greater than zero.");
        }

        if (definition.Sources.Count == 0)
        {
            errors.Add(
                "Rest type must have at least one source reference.");
        }

        return errors;
    }

    public static void EnsureValid(RestTypeDefinition definition)
    {
        IReadOnlyList<string> errors = Validate(definition);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Rest type definition '{definition.Id}' is invalid:" +
            Environment.NewLine +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
