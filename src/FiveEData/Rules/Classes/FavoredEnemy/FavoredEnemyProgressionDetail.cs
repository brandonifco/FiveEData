namespace FiveEData.Rules.Classes.FavoredEnemy;

public sealed record FavoredEnemyProgressionDetail
{
    public FavoredEnemyProgressionDetail(
        IEnumerable<FavoredEnemyChoiceGrant> enemyTypesKnownByLevel,
        bool grantsAssociatedLanguagePerChoice)
    {
        ArgumentNullException.ThrowIfNull(enemyTypesKnownByLevel);

        EnemyTypesKnownByLevel =
            Array.AsReadOnly(enemyTypesKnownByLevel.ToArray());
        GrantsAssociatedLanguagePerChoice = grantsAssociatedLanguagePerChoice;
    }

    public IReadOnlyList<FavoredEnemyChoiceGrant> EnemyTypesKnownByLevel
    {
        get;
    }

    public bool GrantsAssociatedLanguagePerChoice { get; }
}
