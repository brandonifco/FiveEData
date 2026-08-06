namespace FiveEData.Rules.Classes.FontOfMagic;

public readonly record struct FontOfMagicSlotCostGrant
{
    public FontOfMagicSlotCostGrant(int spellSlotLevel, int sorceryPointCost)
    {
        if (spellSlotLevel is < 1 or > 9)
        {
            throw new ArgumentOutOfRangeException(
                nameof(spellSlotLevel),
                spellSlotLevel,
                "Spell slot level must be between 1 and 9.");
        }

        if (sorceryPointCost <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(sorceryPointCost),
                sorceryPointCost,
                "Sorcery point cost must be greater than zero.");
        }

        SpellSlotLevel = spellSlotLevel;
        SorceryPointCost = sorceryPointCost;
    }

    public int SpellSlotLevel { get; }

    public int SorceryPointCost { get; }
}
