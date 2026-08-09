namespace FiveEData.Rules.Adventuring.TravelPace;

internal static class TravelPaceDefinitionValidator
{
    public static IReadOnlyList<string> Validate(
        TravelPaceDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(definition.Id.Value))
        {
            errors.Add("Travel pace ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(definition.Name))
        {
            errors.Add("Travel pace name must not be empty.");
        }

        if (definition.FeetPerMinute <= 0)
        {
            errors.Add(
                "Travel pace feet per minute must be greater than zero.");
        }

        if (definition.MilesPerHour <= 0)
        {
            errors.Add(
                "Travel pace miles per hour must be greater than zero.");
        }

        if (definition.MilesPerDay <= 0)
        {
            errors.Add(
                "Travel pace miles per day must be greater than zero.");
        }

        if (definition.PassiveWisdomPerceptionPenalty is
                { } passiveWisdomPerceptionPenalty &&
            passiveWisdomPerceptionPenalty <= 0)
        {
            errors.Add(
                "Travel pace passive Wisdom (Perception) penalty must be " +
                "greater than zero.");
        }

        if (definition.Sources.Count == 0)
        {
            errors.Add(
                "Travel pace must have at least one source reference.");
        }

        return errors;
    }

    public static void EnsureValid(TravelPaceDefinition definition)
    {
        IReadOnlyList<string> errors = Validate(definition);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Travel pace definition '{definition.Id}' is invalid:" +
            Environment.NewLine +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
