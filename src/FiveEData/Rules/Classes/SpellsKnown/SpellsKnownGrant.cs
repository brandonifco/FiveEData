namespace FiveEData.Rules.Classes.SpellsKnown;

public readonly record struct SpellsKnownGrant
{
    public SpellsKnownGrant(
        int characterLevel,
        int spellsKnown)
    {
        if (characterLevel is < 1 or > 20)
        {
            throw new ArgumentOutOfRangeException(
                nameof(characterLevel),
                characterLevel,
                "Character level must be between 1 and 20.");
        }

        if (spellsKnown <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(spellsKnown),
                spellsKnown,
                "Spells known must be greater than zero.");
        }

        CharacterLevel = characterLevel;
        SpellsKnown = spellsKnown;
    }

    public int CharacterLevel { get; }

    public int SpellsKnown { get; }
}
