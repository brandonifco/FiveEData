using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.FavoredEnemy.Serialization;

internal sealed class FavoredEnemyProgressionDetailData
{
    [JsonRequired]
    public FavoredEnemyChoiceGrantData[]? EnemyTypesKnownByLevel { get; init; }

    [JsonRequired]
    public bool GrantsAssociatedLanguagePerChoice { get; init; }
}

internal sealed class FavoredEnemyChoiceGrantData
{
    [JsonRequired]
    public int CharacterLevel { get; init; }

    [JsonRequired]
    public int EnemyTypesKnown { get; init; }
}
