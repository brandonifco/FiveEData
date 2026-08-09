namespace FiveEData.Rules.Classes.CantripsKnown;

public readonly record struct CantripsKnownGrant
{
    public CantripsKnownGrant(
        int characterLevel,
        int cantripsKnown)
    {
        if (characterLevel is < 1 or > 20)
        {
            throw new ArgumentOutOfRangeException(
                nameof(characterLevel),
                characterLevel,
                "Character level must be between 1 and 20.");
        }

        if (cantripsKnown <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(cantripsKnown),
                cantripsKnown,
                "Cantrips known must be greater than zero.");
        }

        CharacterLevel = characterLevel;
        CantripsKnown = cantripsKnown;
    }

    public int CharacterLevel { get; }

    public int CantripsKnown { get; }
}
