namespace FiveEData.Rules.Equipment.Armor;

public readonly record struct ArmorClassFormula
{
    public ArmorClassFormula(
        int baseArmorClass,
        bool includesDexterityModifier,
        int? maximumDexterityModifier = null)
    {
        if (baseArmorClass <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(baseArmorClass),
                baseArmorClass,
                "Base Armor Class must be greater than zero.");
        }

        if (!includesDexterityModifier && maximumDexterityModifier is not null)
        {
            throw new ArgumentException(
                "A Dexterity modifier maximum requires Dexterity to be included.",
                nameof(maximumDexterityModifier));
        }

        if (maximumDexterityModifier is < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maximumDexterityModifier),
                maximumDexterityModifier,
                "Maximum Dexterity modifier cannot be negative.");
        }

        BaseArmorClass = baseArmorClass;
        IncludesDexterityModifier = includesDexterityModifier;
        MaximumDexterityModifier = maximumDexterityModifier;
    }

    public int BaseArmorClass { get; }
    public bool IncludesDexterityModifier { get; }
    public int? MaximumDexterityModifier { get; }
}
