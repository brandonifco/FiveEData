namespace FiveEData.Rules.Classes.FightingStyles;

internal static class FightingStyleDefinitionValidator
{
    public static IReadOnlyList<string> Validate(
        FightingStyleDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(definition.Id.Value))
        {
            errors.Add("Fighting style ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(definition.Name))
        {
            errors.Add("Fighting style name must not be empty.");
        }

        if (definition.AvailableToClassIds.Count == 0)
        {
            errors.Add(
                "Fighting style must be available to at least one class.");
        }

        var seenClasses = new HashSet<ClassId>();

        foreach (ClassId classId in definition.AvailableToClassIds)
        {
            if (string.IsNullOrWhiteSpace(classId.Value))
            {
                errors.Add(
                    "Fighting style available class ID must not be empty.");
                continue;
            }

            if (!seenClasses.Add(classId))
            {
                errors.Add(
                    $"Fighting style available class '{classId}' is duplicated.");
            }
        }

        int mechanismCount = 0;

        if (definition.RollBonus is not null)
        {
            mechanismCount++;
        }

        if (definition.ArmorClassBonus is { } armorClassBonus)
        {
            mechanismCount++;

            if (armorClassBonus <= 0)
            {
                errors.Add(
                    "Fighting style armor class bonus must be greater " +
                    "than zero.");
            }
        }

        if (definition.DamageDieReroll is not null)
        {
            mechanismCount++;
        }

        if (definition.Reaction is not null)
        {
            mechanismCount++;
        }

        if (definition.GrantsOffHandAbilityModifierDamage)
        {
            mechanismCount++;
        }

        if (mechanismCount != 1)
        {
            errors.Add(
                "Fighting style must have exactly one mechanical " +
                $"effect, but had {mechanismCount}.");
        }

        if (definition.Sources.Count == 0)
        {
            errors.Add(
                "Fighting style must have at least one source reference.");
        }

        return errors;
    }

    public static void EnsureValid(
        FightingStyleDefinition definition)
    {
        IReadOnlyList<string> errors = Validate(definition);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Fighting style definition '{definition.Id}' is invalid:" +
            Environment.NewLine +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
