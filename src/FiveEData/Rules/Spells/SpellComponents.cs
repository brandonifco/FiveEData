namespace FiveEData.Rules.Spells;

public readonly record struct SpellComponents
{
    public SpellComponents(
        bool verbal,
        bool somatic,
        bool material,
        string? materialDescription)
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
        else if (materialDescription is not null)
        {
            throw new ArgumentException(
                "A spell without a material component must not carry a " +
                "material description.",
                nameof(materialDescription));
        }

        Verbal = verbal;
        Somatic = somatic;
        Material = material;
        MaterialDescription = materialDescription;
    }

    public bool Verbal { get; }
    public bool Somatic { get; }
    public bool Material { get; }
    public string? MaterialDescription { get; }
}
