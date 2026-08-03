namespace FiveEData.Rules.Equipment.Ammunition;

internal static class AmmunitionDefinitionValidator
{
    public static IReadOnlyList<string> Validate(AmmunitionDefinition ammunition)
    {
        ArgumentNullException.ThrowIfNull(ammunition);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(ammunition.Id.Value))
        {
            errors.Add("Ammunition ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(ammunition.Name))
        {
            errors.Add("Ammunition name must not be empty.");
        }

        if (ammunition.BundleQuantity <= 0)
        {
            errors.Add("Ammunition bundle quantity must be greater than zero.");
        }

        if (ammunition.Cost.CopperPieces <= 0)
        {
            errors.Add("Ammunition bundle cost must be greater than zero.");
        }

        if (ammunition.Weight.Pounds <= 0)
        {
            errors.Add("Ammunition bundle weight must be greater than zero.");
        }

        if (ammunition.Sources.Count == 0)
        {
            errors.Add("Ammunition must have at least one source reference.");
        }

        return errors;
    }

    public static void EnsureValid(AmmunitionDefinition ammunition)
    {
        IReadOnlyList<string> errors = Validate(ammunition);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Ammunition definition '{ammunition.Id}' is invalid:{Environment.NewLine}" +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
