namespace FiveEData.Rules.Combat.Cover;

internal static class CoverDefinitionValidator
{
    public static IReadOnlyList<string> Validate(CoverDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(definition.Id.Value))
        {
            errors.Add("Cover ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(definition.Name))
        {
            errors.Add("Cover name must not be empty.");
        }

        if (definition.ArmorClassBonus is { } armorClassBonus &&
            armorClassBonus <= 0)
        {
            errors.Add(
                "Cover armor class bonus must be greater than zero.");
        }

        if (definition.DexteritySavingThrowBonus is
                { } dexteritySavingThrowBonus &&
            dexteritySavingThrowBonus <= 0)
        {
            errors.Add(
                "Cover Dexterity saving throw bonus must be greater " +
                "than zero.");
        }

        if (definition.Sources.Count == 0)
        {
            errors.Add("Cover must have at least one source reference.");
        }

        return errors;
    }

    public static void EnsureValid(CoverDefinition definition)
    {
        IReadOnlyList<string> errors = Validate(definition);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Cover definition '{definition.Id}' is invalid:" +
            Environment.NewLine +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
