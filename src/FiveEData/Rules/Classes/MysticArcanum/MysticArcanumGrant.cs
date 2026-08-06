namespace FiveEData.Rules.Classes.MysticArcanum;

public readonly record struct MysticArcanumGrant
{
    public MysticArcanumGrant(int characterLevel, int spellLevel)
    {
        if (characterLevel is < 1 or > 20)
        {
            throw new ArgumentOutOfRangeException(
                nameof(characterLevel),
                characterLevel,
                "Character level must be between 1 and 20.");
        }

        if (spellLevel is < 1 or > 9)
        {
            throw new ArgumentOutOfRangeException(
                nameof(spellLevel),
                spellLevel,
                "Spell level must be between 1 and 9.");
        }

        CharacterLevel = characterLevel;
        SpellLevel = spellLevel;
    }

    public int CharacterLevel { get; }

    public int SpellLevel { get; }
}
