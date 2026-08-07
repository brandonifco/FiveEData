namespace FiveEData.Rules.Classes.UnarmoredMovement;

public readonly record struct UnarmoredMovementSpeedBonusGrant
{
    public UnarmoredMovementSpeedBonusGrant(
        int characterLevel,
        int speedBonusFeet)
    {
        if (characterLevel is < 1 or > 20)
        {
            throw new ArgumentOutOfRangeException(
                nameof(characterLevel),
                characterLevel,
                "Character level must be between 1 and 20.");
        }

        if (speedBonusFeet <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(speedBonusFeet),
                speedBonusFeet,
                "Unarmored Movement speed bonus must be greater than zero.");
        }

        CharacterLevel = characterLevel;
        SpeedBonusFeet = speedBonusFeet;
    }

    public int CharacterLevel { get; }

    public int SpeedBonusFeet { get; }
}
