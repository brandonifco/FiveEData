namespace FiveEData.Rules.Classes.FavoredEnemy;

public readonly record struct FavoredEnemyChoiceGrant
{
    public FavoredEnemyChoiceGrant(int characterLevel, int enemyTypesKnown)
    {
        if (characterLevel is < 1 or > 20)
        {
            throw new ArgumentOutOfRangeException(
                nameof(characterLevel),
                characterLevel,
                "Character level must be between 1 and 20.");
        }

        if (enemyTypesKnown <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(enemyTypesKnown),
                enemyTypesKnown,
                "Favored enemy types known must be greater than zero.");
        }

        CharacterLevel = characterLevel;
        EnemyTypesKnown = enemyTypesKnown;
    }

    public int CharacterLevel { get; }

    public int EnemyTypesKnown { get; }
}
