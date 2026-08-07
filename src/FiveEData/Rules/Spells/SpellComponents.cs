namespace FiveEData.Rules.Spells;

public readonly record struct SpellComponents
{
    public SpellComponents(
        bool verbal,
        bool somatic,
        bool material,
        string? materialDescription,
        int? materialCostGoldPieces = null,
        bool materialIsConsumed = false)
    {
        if (!verbal && !somatic && !material)
        {
            throw new ArgumentException(
                "A spell must require at least one component.",
                nameof(verbal));
        }

        if (material)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(
                materialDescription,
                nameof(materialDescription));
        }
        else
        {
            if (materialDescription is not null)
            {
                throw new ArgumentException(
                    "A spell without a material component must not carry " +
                    "a material description.",
                    nameof(materialDescription));
            }

            if (materialCostGoldPieces is not null)
            {
                throw new ArgumentException(
                    "A spell without a material component must not carry " +
                    "a material cost.",
                    nameof(materialCostGoldPieces));
            }

            if (materialIsConsumed)
            {
                throw new ArgumentException(
                    "A spell without a material component cannot consume " +
                    "one.",
                    nameof(materialIsConsumed));
            }
        }

        if (materialCostGoldPieces is <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(materialCostGoldPieces),
                materialCostGoldPieces,
                "Spell material cost must be greater than zero when " +
                "specified.");
        }

        Verbal = verbal;
        Somatic = somatic;
        Material = material;
        MaterialDescription = materialDescription;
        MaterialCostGoldPieces = materialCostGoldPieces;
        MaterialIsConsumed = materialIsConsumed;
    }

    public bool Verbal { get; }
    public bool Somatic { get; }
    public bool Material { get; }
    public string? MaterialDescription { get; }

    /// <summary>
    /// Minimum gold-piece value the PHB states for a material component,
    /// as in Chromatic Orb's "a diamond worth at least 50 gp". Null when
    /// the component carries no stated cost.
    /// </summary>
    public int? MaterialCostGoldPieces { get; }

    /// <summary>
    /// True when the PHB states the spell consumes its material component,
    /// as in Find Familiar's charcoal and incense "that must be consumed
    /// by fire".
    /// </summary>
    public bool MaterialIsConsumed { get; }
}
