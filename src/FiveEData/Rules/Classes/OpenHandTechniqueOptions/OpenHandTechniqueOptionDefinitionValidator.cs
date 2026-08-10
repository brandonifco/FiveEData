namespace FiveEData.Rules.Classes.OpenHandTechniqueOptions;

internal static class OpenHandTechniqueOptionDefinitionValidator
{
    public static IReadOnlyList<string> Validate(
        OpenHandTechniqueOptionDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(definition.Id.Value))
        {
            errors.Add("Open hand technique option ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(definition.Name))
        {
            errors.Add("Open hand technique option name must not be empty.");
        }

        if (definition.PushDistanceFeet is { } pushDistanceFeet &&
            pushDistanceFeet <= 0)
        {
            errors.Add(
                "Open hand technique option push distance must be greater " +
                "than zero.");
        }

        bool hasSavingThrow = definition.SavingThrowAbilityId is not null;

        if (definition.ImposedConditionId is not null && !hasSavingThrow)
        {
            errors.Add(
                "Open hand technique option cannot impose a condition " +
                "without a saving throw.");
        }

        if (definition.PushDistanceFeet is not null && !hasSavingThrow)
        {
            errors.Add(
                "Open hand technique option cannot push without a saving " +
                "throw.");
        }

        if (definition.PreventsReactionsUntil is not null &&
            !definition.PreventsReactions)
        {
            errors.Add(
                "Open hand technique option cannot bound reaction " +
                "prevention it does not impose.");
        }

        if (definition.PreventsReactionsUntil is { } preventsReactionsUntil &&
            !Enum.IsDefined(preventsReactionsUntil))
        {
            errors.Add(
                "Open hand technique option reaction prevention duration " +
                "must be defined.");
        }

        if (definition.Sources.Count == 0)
        {
            errors.Add(
                "Open hand technique option must have at least one source " +
                "reference.");
        }

        return errors;
    }

    public static void EnsureValid(
        OpenHandTechniqueOptionDefinition definition)
    {
        IReadOnlyList<string> errors = Validate(definition);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Open hand technique option definition '{definition.Id}' is " +
            $"invalid:{Environment.NewLine}" +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
