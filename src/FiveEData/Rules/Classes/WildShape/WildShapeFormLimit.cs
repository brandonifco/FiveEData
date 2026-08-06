namespace FiveEData.Rules.Classes.WildShape;

public readonly record struct WildShapeFormLimit
{
    public WildShapeFormLimit(
        int characterLevel,
        double maxChallengeRating,
        bool allowsFlyingSpeed,
        bool allowsSwimmingSpeed)
    {
        if (characterLevel is < 1 or > 20)
        {
            throw new ArgumentOutOfRangeException(
                nameof(characterLevel),
                characterLevel,
                "Character level must be between 1 and 20.");
        }

        if (maxChallengeRating <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxChallengeRating),
                maxChallengeRating,
                "Max challenge rating must be greater than zero.");
        }

        CharacterLevel = characterLevel;
        MaxChallengeRating = maxChallengeRating;
        AllowsFlyingSpeed = allowsFlyingSpeed;
        AllowsSwimmingSpeed = allowsSwimmingSpeed;
    }

    public int CharacterLevel { get; }

    public double MaxChallengeRating { get; }

    public bool AllowsFlyingSpeed { get; }

    public bool AllowsSwimmingSpeed { get; }
}
