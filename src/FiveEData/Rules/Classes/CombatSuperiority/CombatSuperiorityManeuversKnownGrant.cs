namespace FiveEData.Rules.Classes.CombatSuperiority;

public readonly record struct CombatSuperiorityManeuversKnownGrant
{
    public CombatSuperiorityManeuversKnownGrant(
        int characterLevel,
        int maneuversKnown)
    {
        if (characterLevel is < 1 or > 20)
        {
            throw new ArgumentOutOfRangeException(
                nameof(characterLevel),
                characterLevel,
                "Character level must be between 1 and 20.");
        }

        if (maneuversKnown <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maneuversKnown),
                maneuversKnown,
                "Maneuvers known must be greater than zero.");
        }

        CharacterLevel = characterLevel;
        ManeuversKnown = maneuversKnown;
    }

    public int CharacterLevel { get; }

    public int ManeuversKnown { get; }
}
