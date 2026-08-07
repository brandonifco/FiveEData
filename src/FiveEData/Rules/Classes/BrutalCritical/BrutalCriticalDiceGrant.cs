namespace FiveEData.Rules.Classes.BrutalCritical;

public readonly record struct BrutalCriticalDiceGrant
{
    public BrutalCriticalDiceGrant(int characterLevel, int additionalDice)
    {
        if (characterLevel is < 1 or > 20)
        {
            throw new ArgumentOutOfRangeException(
                nameof(characterLevel),
                characterLevel,
                "Character level must be between 1 and 20.");
        }

        if (additionalDice <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(additionalDice),
                additionalDice,
                "Brutal Critical additional dice must be greater than zero.");
        }

        CharacterLevel = characterLevel;
        AdditionalDice = additionalDice;
    }

    public int CharacterLevel { get; }

    public int AdditionalDice { get; }
}
