namespace FiveEData.Rules.Spells;

internal static class SpellDefinitionValidator
{
    private const int MinimumLevel = 0;
    private const int MaximumLevel = 9;

    public static IReadOnlyList<string> Validate(SpellDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(definition.Id.Value))
        {
            errors.Add("Spell ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(definition.Name))
        {
            errors.Add("Spell name must not be empty.");
        }

        if (definition.Level is < MinimumLevel or > MaximumLevel)
        {
            errors.Add(
                $"Spell level must be between {MinimumLevel} and " +
                $"{MaximumLevel}.");
        }

        if (string.IsNullOrWhiteSpace(definition.SchoolId.Value))
        {
            errors.Add("Spell must reference a magic school.");
        }

        ValidateRange(definition, errors);
        ValidateDuration(definition, errors);

        if (definition.AvailableToClassIds.Count == 0)
        {
            errors.Add("Spell must be available to at least one class.");
        }

        if (definition.AvailableToClassIds.Distinct().Count()
            != definition.AvailableToClassIds.Count)
        {
            errors.Add("Spell must not repeat a class in its class list.");
        }

        if (definition.Sources.Count == 0)
        {
            errors.Add("Spell must have at least one source reference.");
        }

        return errors;
    }

    private static void ValidateRange(
        SpellDefinition definition,
        List<string> errors)
    {
        bool isDistance = definition.Range.Kind == SpellRangeKind.Distance;

        if (isDistance && definition.Range.DistanceFeet is null)
        {
            errors.Add("A ranged spell must specify a distance in feet.");
        }

        if (!isDistance && definition.Range.DistanceFeet is not null)
        {
            errors.Add(
                "A self or touch spell must not specify a distance.");
        }
    }

    private static void ValidateDuration(
        SpellDefinition definition,
        List<string> errors)
    {
        SpellDuration duration = definition.Duration;

        if (duration.IsInstantaneous)
        {
            if (duration.Amount is not null || duration.Unit is not null)
            {
                errors.Add(
                    "An instantaneous spell must not specify a duration " +
                    "amount or unit.");
            }

            if (duration.RequiresConcentration || duration.IsUpTo)
            {
                errors.Add(
                    "An instantaneous spell must not require " +
                    "concentration or be an \"up to\" duration.");
            }

            return;
        }

        if (duration.Amount is null || duration.Unit is null)
        {
            errors.Add(
                "A non-instantaneous spell must specify a duration " +
                "amount and unit.");
        }

        if (duration.RequiresConcentration && !duration.IsUpTo)
        {
            errors.Add(
                "A concentration duration is always an \"up to\" " +
                "duration.");
        }
    }

    public static void EnsureValid(SpellDefinition definition)
    {
        IReadOnlyList<string> errors = Validate(definition);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Spell definition '{definition.Id}' is invalid:" +
            Environment.NewLine +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
