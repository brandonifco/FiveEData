namespace FiveEData.Rules.Classes.Metamagic;

internal static class MetamagicOptionDefinitionValidator
{
    public static IReadOnlyList<string> Validate(
        MetamagicOptionDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(definition.Id.Value))
        {
            errors.Add("Metamagic option ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(definition.Name))
        {
            errors.Add("Metamagic option name must not be empty.");
        }

        int costRepresentationCount = 0;

        if (definition.FixedSorceryPointCost is { } fixedCost)
        {
            costRepresentationCount++;

            if (fixedCost <= 0)
            {
                errors.Add(
                    "Metamagic option fixed sorcery point cost must be " +
                    "greater than zero.");
            }
        }

        if (definition.CostEqualsSpellLevelWithCantripMinimum)
        {
            costRepresentationCount++;
        }

        if (costRepresentationCount != 1)
        {
            errors.Add(
                "Metamagic option must have exactly one cost " +
                $"representation, but had {costRepresentationCount}.");
        }

        if (definition.Sources.Count == 0)
        {
            errors.Add(
                "Metamagic option must have at least one source " +
                "reference.");
        }

        return errors;
    }

    public static void EnsureValid(MetamagicOptionDefinition definition)
    {
        IReadOnlyList<string> errors = Validate(definition);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Metamagic option definition '{definition.Id}' is invalid:" +
            Environment.NewLine +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
