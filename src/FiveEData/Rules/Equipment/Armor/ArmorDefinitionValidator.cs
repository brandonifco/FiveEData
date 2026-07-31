namespace FiveEData.Rules.Equipment.Armor;

internal static class ArmorDefinitionValidator
{
    public static IReadOnlyList<string> Validate(ArmorDefinition armor)
    {
        ArgumentNullException.ThrowIfNull(armor);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(armor.Id.Value))
        {
            errors.Add("Armor ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(armor.Name))
        {
            errors.Add("Armor name must not be empty.");
        }

        if (armor.Cost.CopperPieces <= 0)
        {
            errors.Add("Armor cost must be greater than zero.");
        }

        if (armor.Weight.Pounds <= 0)
        {
            errors.Add("Armor weight must be greater than zero.");
        }

        if (armor.MinimumStrengthForFullSpeed is <= 0)
        {
            errors.Add(
                "Minimum Strength for full speed must be greater than zero when specified.");
        }

        if (armor.ArmorClass.BaseArmorClass <= 0)
        {
            errors.Add("Base Armor Class must be greater than zero.");
        }

        switch (armor.Category)
        {
            case ArmorCategory.Light:
                if (!armor.ArmorClass.IncludesDexterityModifier ||
                    armor.ArmorClass.MaximumDexterityModifier is not null)
                {
                    errors.Add(
                        "D&D 5e 2014 light armor must include the full Dexterity modifier.");
                }

                if (armor.MinimumStrengthForFullSpeed is not null)
                {
                    errors.Add(
                        "D&D 5e 2014 light armor cannot define a Strength threshold.");
                }

                break;

            case ArmorCategory.Medium:
                if (!armor.ArmorClass.IncludesDexterityModifier ||
                    armor.ArmorClass.MaximumDexterityModifier != 2)
                {
                    errors.Add(
                        "D&D 5e 2014 medium armor must include the Dexterity modifier with a maximum of +2.");
                }

                if (armor.MinimumStrengthForFullSpeed is not null)
                {
                    errors.Add(
                        "D&D 5e 2014 medium armor cannot define a Strength threshold.");
                }

                break;

            case ArmorCategory.Heavy:
                if (armor.ArmorClass.IncludesDexterityModifier ||
                    armor.ArmorClass.MaximumDexterityModifier is not null)
                {
                    errors.Add(
                        "D&D 5e 2014 heavy armor cannot include the Dexterity modifier.");
                }

                break;

            default:
                errors.Add("Armor category is not recognized.");
                break;
        }

        if (armor.Sources.Count == 0)
        {
            errors.Add("Armor must have at least one source reference.");
        }

        return errors;
    }

    public static void EnsureValid(ArmorDefinition armor)
    {
        IReadOnlyList<string> errors = Validate(armor);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Armor definition '{armor.Id}' is invalid:{Environment.NewLine}" +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
