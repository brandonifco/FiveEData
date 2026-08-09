namespace FiveEData.Rules.Classes.WizardSpellbook;

public sealed record WizardSpellbookDetail
{
    public WizardSpellbookDetail(
        int startingSpellCount,
        int spellsAddedPerLevelAfterFirst)
    {
        if (startingSpellCount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(startingSpellCount),
                startingSpellCount,
                "Starting spell count must be greater than zero.");
        }

        if (spellsAddedPerLevelAfterFirst <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(spellsAddedPerLevelAfterFirst),
                spellsAddedPerLevelAfterFirst,
                "Spells added per level after first must be greater than " +
                "zero.");
        }

        StartingSpellCount = startingSpellCount;
        SpellsAddedPerLevelAfterFirst = spellsAddedPerLevelAfterFirst;
    }

    public int StartingSpellCount { get; }

    public int SpellsAddedPerLevelAfterFirst { get; }
}
