using FiveEData.Rules.Creatures.DamageTypes;

namespace FiveEData.Rules.Classes.TransmutersStoneOptions;

internal static class TransmutersStoneOptionDefinitionValidator
{
    public static IReadOnlyList<string> Validate(
        TransmutersStoneOptionDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(definition.Id.Value))
        {
            errors.Add("Transmuter's stone option ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(definition.Name))
        {
            errors.Add("Transmuter's stone option name must not be empty.");
        }

        if (definition.DarkvisionRangeFeet is { } darkvisionRangeFeet &&
            darkvisionRangeFeet <= 0)
        {
            errors.Add(
                "Transmuter's stone option darkvision range must be greater " +
                "than zero.");
        }

        if (definition.SpeedBonusFeet is { } speedBonusFeet &&
            speedBonusFeet <= 0)
        {
            errors.Add(
                "Transmuter's stone option speed bonus must be greater than " +
                "zero.");
        }

        if (definition.RequiresUnencumbered &&
            definition.SpeedBonusFeet is null)
        {
            errors.Add(
                "Transmuter's stone option cannot require being " +
                "unencumbered without a speed bonus.");
        }

        if (definition.ChoosableResistedDamageTypeIds.Count == 1)
        {
            errors.Add(
                "Transmuter's stone option choosable resisted damage types " +
                "must offer at least two options.");
        }

        var seenDamageTypeIds = new HashSet<DamageTypeId>();

        foreach (
            DamageTypeId damageTypeId
            in definition.ChoosableResistedDamageTypeIds)
        {
            if (!seenDamageTypeIds.Add(damageTypeId))
            {
                errors.Add(
                    $"Transmuter's stone option lists duplicate choosable " +
                    $"resisted damage type '{damageTypeId}'.");
            }
        }

        if (definition.Sources.Count == 0)
        {
            errors.Add(
                "Transmuter's stone option must have at least one source " +
                "reference.");
        }

        return errors;
    }

    public static void EnsureValid(
        TransmutersStoneOptionDefinition definition)
    {
        IReadOnlyList<string> errors = Validate(definition);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Transmuter's stone option definition '{definition.Id}' is " +
            $"invalid:{Environment.NewLine}" +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
