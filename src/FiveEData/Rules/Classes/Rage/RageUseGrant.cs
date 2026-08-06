namespace FiveEData.Rules.Classes.Rage;

public readonly record struct RageUseGrant
{
    public RageUseGrant(int characterLevel, int? usesPerLongRest)
    {
        if (characterLevel is < 1 or > 20)
        {
            throw new ArgumentOutOfRangeException(
                nameof(characterLevel),
                characterLevel,
                "Character level must be between 1 and 20.");
        }

        if (usesPerLongRest is <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(usesPerLongRest),
                usesPerLongRest,
                "Rage uses per long rest must be greater than zero, " +
                "or null to represent an unlimited number of uses.");
        }

        CharacterLevel = characterLevel;
        UsesPerLongRest = usesPerLongRest;
    }

    public int CharacterLevel { get; }

    public int? UsesPerLongRest { get; }
}
