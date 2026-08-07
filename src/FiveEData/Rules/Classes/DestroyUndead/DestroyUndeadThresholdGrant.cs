namespace FiveEData.Rules.Classes.DestroyUndead;

public readonly record struct DestroyUndeadThresholdGrant
{
    public DestroyUndeadThresholdGrant(
        int characterLevel,
        double maxChallengeRating)
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
    }

    public int CharacterLevel { get; }

    public double MaxChallengeRating { get; }
}
