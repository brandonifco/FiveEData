namespace FiveEData.Rules.Equipment.Weapons;

internal static class WeaponDefinitionValidator
{
    public static IReadOnlyList<string> Validate(WeaponDefinition weapon)
    {
        ArgumentNullException.ThrowIfNull(weapon);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(weapon.Name))
        {
            errors.Add("Weapon name must not be empty.");
        }

        if (weapon.Sources.Count == 0)
        {
            errors.Add("Weapon must have at least one source reference.");
        }

        bool hasAmmunition = weapon.Properties.Contains(WeaponProperty.Ammunition);
        bool hasHeavy = weapon.Properties.Contains(WeaponProperty.Heavy);
        bool hasLight = weapon.Properties.Contains(WeaponProperty.Light);
        bool hasLoading = weapon.Properties.Contains(WeaponProperty.Loading);
        bool hasSpecial = weapon.Properties.Contains(WeaponProperty.Special);
        bool hasThrown = weapon.Properties.Contains(WeaponProperty.Thrown);
        bool hasTwoHanded = weapon.Properties.Contains(WeaponProperty.TwoHanded);
        bool hasVersatile = weapon.Properties.Contains(WeaponProperty.Versatile);


        if (hasThrown && weapon.Range is null)
        {
            errors.Add("A weapon with the Thrown property must define a range.");
        }

        if (hasAmmunition && weapon.Range is null)
        {
            errors.Add("A weapon with the Ammunition property must define a range.");
        }

        if (weapon.Range is not null && !hasThrown && !hasAmmunition)
        {
            errors.Add(
                "A weapon range requires either the Thrown or Ammunition property.");
        }

        if (weapon.Range is not null &&
            weapon.Range.Long.Feet <= weapon.Range.Normal.Feet)
        {
            errors.Add(
                "For a D&D 5e 2014 weapon, long range must be greater than normal range.");
        }

        if (hasAmmunition && weapon.AmmunitionTypeId is null)
        {
            errors.Add(
                "A weapon with the Ammunition property must define an ammunition type.");
        }

        if (!hasAmmunition && weapon.AmmunitionTypeId is not null)
        {
            errors.Add(
                "A weapon without the Ammunition property cannot define an ammunition type.");
        }

        if (hasAmmunition &&
            weapon.UsageCategory != WeaponUsageCategory.Ranged)
        {
            errors.Add(
                "A weapon with the Ammunition property must be classified as ranged.");
        }

        if (hasLoading && !hasAmmunition)
        {
            errors.Add(
                "A weapon with the Loading property must also have the Ammunition property.");
        }

        if (hasVersatile && weapon.VersatileDamage is null)
        {
            errors.Add(
                "A weapon with the Versatile property must define versatile damage.");
        }

        if (!hasVersatile && weapon.VersatileDamage is not null)
        {
            errors.Add(
                "Versatile damage requires the Versatile property.");
        }

        if (hasVersatile &&
            weapon.UsageCategory != WeaponUsageCategory.Melee)
        {
            errors.Add(
                "A weapon with the Versatile property must be classified as melee.");
        }

        if (hasVersatile &&
            weapon.Damage?.Dice is null)
        {
            errors.Add(
                "A versatile weapon must define ordinary dice-based weapon damage.");
        }

        if (hasSpecial && weapon.SpecialRuleIds.Count == 0)
        {
            errors.Add(
                "A weapon with the Special property must reference at least one special rule.");
        }

        if (!hasSpecial && weapon.SpecialRuleIds.Count != 0)
        {
            errors.Add(
                "Special rule references require the Special property.");
        }

        if (hasLight && hasHeavy)
        {
            errors.Add(
                "A weapon cannot have both the Light and Heavy properties.");
        }

        if (hasTwoHanded && hasVersatile)
        {
            errors.Add(
                "A weapon cannot have both the TwoHanded and Versatile properties.");
        }

        if (weapon.UsageCategory == WeaponUsageCategory.Ranged &&
            !hasAmmunition &&
            !hasThrown)
        {
            errors.Add(
                "A D&D 5e 2014 ranged weapon must have either the Ammunition or Thrown property.");
        }

        return errors;
    }

    public static void EnsureValid(WeaponDefinition weapon)
    {
        IReadOnlyList<string> errors = Validate(weapon);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Weapon definition '{weapon.Id}' is invalid:{Environment.NewLine}" +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }
}
