namespace FiveEData.Rules.Equipment.Shields;

internal static class ShieldDefinitionValidator
{
    public static IReadOnlyList<string> Validate(ShieldDefinition shield)
    {
        ArgumentNullException.ThrowIfNull(shield);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(shield.Id.Value))
        {
            errors.Add("Shield ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(shield.Name))
        {
            errors.Add("Shield name must not be empty.");
        }

        if (shield.Cost.CopperPieces <= 0)
        {
            errors.Add("Shield cost must be greater than zero.");
        }

        if (shield.Weight.Pounds <= 0)
        {
            errors.Add("Shield weight must be greater than zero.");
        }

        if (shield.ArmorClassBonus <= 0)
        {
            errors.Add("Shield Armor Class bonus must be greater than zero.");
        }

        if (shield.Sources.Count == 0)
        {
            errors.Add("Shield must have at least one source reference.");
        }

        return errors;
    }

    public static void EnsureValid(ShieldDefinition shield)
    {
        IReadOnlyList<string> errors = Validate(shield);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Shield definition '{shield.Id}' is invalid:{Environment.NewLine}" +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
