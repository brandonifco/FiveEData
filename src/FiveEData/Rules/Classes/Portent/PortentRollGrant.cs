namespace FiveEData.Rules.Classes.Portent;

public readonly record struct PortentRollGrant
{
    public PortentRollGrant(int characterLevel, int foretellingRolls)
    {
        if (characterLevel is < 1 or > 20)
        {
            throw new ArgumentOutOfRangeException(
                nameof(characterLevel),
                characterLevel,
                "Character level must be between 1 and 20.");
        }

        if (foretellingRolls <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(foretellingRolls),
                foretellingRolls,
                "Portent foretelling rolls must be greater than zero.");
        }

        CharacterLevel = characterLevel;
        ForetellingRolls = foretellingRolls;
    }

    public int CharacterLevel { get; }

    public int ForetellingRolls { get; }
}
