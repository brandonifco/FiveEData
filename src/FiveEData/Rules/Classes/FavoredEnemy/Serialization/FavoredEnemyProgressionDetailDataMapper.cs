namespace FiveEData.Rules.Classes.FavoredEnemy.Serialization;

internal static class FavoredEnemyProgressionDetailDataMapper
{
    public static FavoredEnemyProgressionDetail Map(
        FavoredEnemyProgressionDetailData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        FavoredEnemyChoiceGrantData[] grantData =
            data.EnemyTypesKnownByLevel
            ?? throw new ArgumentException(
                "Favored Enemy progression enemy types known by level is " +
                "required.",
                nameof(data));

        FavoredEnemyChoiceGrant[] enemyTypesKnownByLevel = grantData
            .Select(MapGrant)
            .ToArray();

        return new FavoredEnemyProgressionDetail(
            enemyTypesKnownByLevel,
            data.GrantsAssociatedLanguagePerChoice);
    }

    private static FavoredEnemyChoiceGrant MapGrant(
        FavoredEnemyChoiceGrantData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        return new FavoredEnemyChoiceGrant(
            data.CharacterLevel,
            data.EnemyTypesKnown);
    }
}
