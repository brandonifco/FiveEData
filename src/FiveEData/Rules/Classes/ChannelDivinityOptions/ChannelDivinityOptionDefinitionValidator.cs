namespace FiveEData.Rules.Classes.ChannelDivinityOptions;

internal static class ChannelDivinityOptionDefinitionValidator
{
    public static IReadOnlyList<string> Validate(
        ChannelDivinityOptionDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(definition.Id.Value))
        {
            errors.Add("Channel Divinity option ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(definition.Name))
        {
            errors.Add("Channel Divinity option name must not be empty.");
        }

        if (definition.RangeFeet is { } rangeFeet && rangeFeet <= 0)
        {
            errors.Add(
                "Channel Divinity option range must be greater than " +
                "zero feet.");
        }

        if (definition.DurationMinutes is { } durationMinutes &&
            durationMinutes <= 0)
        {
            errors.Add(
                "Channel Divinity option duration must be greater than " +
                "zero minutes.");
        }

        if (definition.RollBonus is { } rollBonus && rollBonus <= 0)
        {
            errors.Add(
                "Channel Divinity option roll bonus must be greater than " +
                "zero.");
        }

        if (definition.Sources.Count == 0)
        {
            errors.Add(
                "Channel Divinity option must have at least one source " +
                "reference.");
        }

        return errors;
    }

    public static void EnsureValid(ChannelDivinityOptionDefinition definition)
    {
        IReadOnlyList<string> errors = Validate(definition);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Channel Divinity option definition '{definition.Id}' is " +
            $"invalid:{Environment.NewLine}" +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
