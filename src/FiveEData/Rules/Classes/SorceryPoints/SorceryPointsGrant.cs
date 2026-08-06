namespace FiveEData.Rules.Classes.SorceryPoints;

public readonly record struct SorceryPointsGrant
{
    public SorceryPointsGrant(int characterLevel, int points)
    {
        if (characterLevel is < 1 or > 20)
        {
            throw new ArgumentOutOfRangeException(
                nameof(characterLevel),
                characterLevel,
                "Character level must be between 1 and 20.");
        }

        if (points <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(points),
                points,
                "Sorcery points must be greater than zero.");
        }

        CharacterLevel = characterLevel;
        Points = points;
    }

    public int CharacterLevel { get; }

    public int Points { get; }
}
