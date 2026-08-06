namespace FiveEData.Rules.Classes.CombatSuperiority;

public readonly record struct CombatSuperiorityDiceCountGrant
{
    public CombatSuperiorityDiceCountGrant(int characterLevel, int diceCount)
    {
        if (characterLevel is < 1 or > 20)
        {
            throw new ArgumentOutOfRangeException(
                nameof(characterLevel),
                characterLevel,
                "Character level must be between 1 and 20.");
        }

        if (diceCount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(diceCount),
                diceCount,
                "Dice count must be greater than zero.");
        }

        CharacterLevel = characterLevel;
        DiceCount = diceCount;
    }

    public int CharacterLevel { get; }

    public int DiceCount { get; }
}
