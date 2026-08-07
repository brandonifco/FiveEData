namespace FiveEData.Rules.Classes.ImprovedCritical;

public readonly record struct CriticalHitThresholdGrant
{
    public CriticalHitThresholdGrant(int characterLevel, int minimumRoll)
    {
        if (characterLevel is < 1 or > 20)
        {
            throw new ArgumentOutOfRangeException(
                nameof(characterLevel),
                characterLevel,
                "Character level must be between 1 and 20.");
        }

        if (minimumRoll is < 2 or > 20)
        {
            throw new ArgumentOutOfRangeException(
                nameof(minimumRoll),
                minimumRoll,
                "Critical hit minimum roll must be between 2 and 20.");
        }

        CharacterLevel = characterLevel;
        MinimumRoll = minimumRoll;
    }

    public int CharacterLevel { get; }

    public int MinimumRoll { get; }
}
