namespace FiveEData.Rules.Classes.DiscipleOfTheElements;

public readonly record struct DiscipleOfTheElementsMaxKiPointsGrant
{
    public DiscipleOfTheElementsMaxKiPointsGrant(
        int characterLevel,
        int maxKiPoints)
    {
        if (characterLevel is < 1 or > 20)
        {
            throw new ArgumentOutOfRangeException(
                nameof(characterLevel),
                characterLevel,
                "Character level must be between 1 and 20.");
        }

        if (maxKiPoints <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxKiPoints),
                maxKiPoints,
                "Max ki points must be greater than zero.");
        }

        CharacterLevel = characterLevel;
        MaxKiPoints = maxKiPoints;
    }

    public int CharacterLevel { get; }

    public int MaxKiPoints { get; }
}
