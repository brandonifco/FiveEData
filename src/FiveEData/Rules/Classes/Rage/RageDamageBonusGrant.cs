namespace FiveEData.Rules.Classes.Rage;

public readonly record struct RageDamageBonusGrant
{
    public RageDamageBonusGrant(int characterLevel, int bonus)
    {
        if (characterLevel is < 1 or > 20)
        {
            throw new ArgumentOutOfRangeException(
                nameof(characterLevel),
                characterLevel,
                "Character level must be between 1 and 20.");
        }

        if (bonus <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(bonus),
                bonus,
                "Rage damage bonus must be greater than zero.");
        }

        CharacterLevel = characterLevel;
        Bonus = bonus;
    }

    public int CharacterLevel { get; }

    public int Bonus { get; }
}
