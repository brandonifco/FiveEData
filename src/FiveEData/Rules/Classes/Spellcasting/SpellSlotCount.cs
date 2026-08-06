namespace FiveEData.Rules.Classes.Spellcasting;

public readonly record struct SpellSlotCount
{
    public SpellSlotCount(int spellLevel, int count)
    {
        if (spellLevel is < 1 or > 9)
        {
            throw new ArgumentOutOfRangeException(
                nameof(spellLevel),
                spellLevel,
                "Spell level must be between 1 and 9.");
        }

        if (count <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(count),
                count,
                "Spell slot count must be greater than zero.");
        }

        SpellLevel = spellLevel;
        Count = count;
    }

    public int SpellLevel { get; }

    public int Count { get; }
}
