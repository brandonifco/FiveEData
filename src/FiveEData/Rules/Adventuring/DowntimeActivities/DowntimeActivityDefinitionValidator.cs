namespace FiveEData.Rules.Adventuring.DowntimeActivities;

internal static class DowntimeActivityDefinitionValidator
{
    public static IReadOnlyList<string> Validate(
        DowntimeActivityDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(definition.Id.Value))
        {
            errors.Add("Downtime activity ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(definition.Name))
        {
            errors.Add("Downtime activity name must not be empty.");
        }

        if (definition.RequiredDays is { } requiredDays &&
            requiredDays <= 0)
        {
            errors.Add(
                "Downtime activity required days must be greater than " +
                "zero.");
        }

        if (definition.CostPerDayGoldPieces is
                { } costPerDayGoldPieces &&
            costPerDayGoldPieces <= 0)
        {
            errors.Add(
                "Downtime activity cost per day must be greater than " +
                "zero.");
        }

        if (definition.SavingThrowDC is { } savingThrowDC &&
            savingThrowDC <= 0)
        {
            errors.Add(
                "Downtime activity saving throw DC must be greater than " +
                "zero.");
        }

        bool hasSavingThrowAbility = definition.SavingThrowAbilityId is not null;
        bool hasSavingThrowDC = definition.SavingThrowDC is not null;

        if (hasSavingThrowAbility != hasSavingThrowDC)
        {
            errors.Add(
                "Downtime activity saving throw ability and DC must be " +
                "both present or both absent.");
        }

        if (definition.MarketValueProgressPerDayGoldPieces is
                { } marketValueProgressPerDayGoldPieces &&
            marketValueProgressPerDayGoldPieces <= 0)
        {
            errors.Add(
                "Downtime activity market value progress per day must " +
                "be greater than zero.");
        }

        if (definition.Sources.Count == 0)
        {
            errors.Add(
                "Downtime activity must have at least one source " +
                "reference.");
        }

        return errors;
    }

    public static void EnsureValid(DowntimeActivityDefinition definition)
    {
        IReadOnlyList<string> errors = Validate(definition);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Downtime activity definition '{definition.Id}' is invalid:" +
            Environment.NewLine +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
