namespace FiveEData.Rules.Classes.Indomitable;

public readonly record struct IndomitableUseGrant
{
    public IndomitableUseGrant(int characterLevel, int usesPerRest)
    {
        if (characterLevel is < 1 or > 20)
        {
            throw new ArgumentOutOfRangeException(
                nameof(characterLevel),
                characterLevel,
                "Character level must be between 1 and 20.");
        }

        if (usesPerRest <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(usesPerRest),
                usesPerRest,
                "Indomitable uses per rest must be greater than zero.");
        }

        CharacterLevel = characterLevel;
        UsesPerRest = usesPerRest;
    }

    public int CharacterLevel { get; }

    public int UsesPerRest { get; }
}
