using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.AlterMemories.Serialization;

internal sealed class AlterMemoriesDetailData
{
    [JsonRequired]
    public bool MakesCreatureUnawareOfCharm { get; init; }

    [JsonRequired]
    public string? ForgetSavingThrowAbilityId { get; init; }
}
