namespace FiveEData.Rules.Classes.DiscipleOfTheElements;

public readonly record struct DiscipleOfTheElementsDisciplinesKnownGrant
{
    public DiscipleOfTheElementsDisciplinesKnownGrant(
        int characterLevel,
        int disciplinesKnown)
    {
        if (characterLevel is < 1 or > 20)
        {
            throw new ArgumentOutOfRangeException(
                nameof(characterLevel),
                characterLevel,
                "Character level must be between 1 and 20.");
        }

        if (disciplinesKnown <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(disciplinesKnown),
                disciplinesKnown,
                "Disciplines known must be greater than zero.");
        }

        CharacterLevel = characterLevel;
        DisciplinesKnown = disciplinesKnown;
    }

    public int CharacterLevel { get; }

    public int DisciplinesKnown { get; }
}
