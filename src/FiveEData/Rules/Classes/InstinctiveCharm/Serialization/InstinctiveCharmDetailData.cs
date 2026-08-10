using System.Text.Json.Serialization;

namespace FiveEData.Rules.Classes.InstinctiveCharm.Serialization;

internal sealed class InstinctiveCharmDetailData
{
    [JsonRequired]
    public int RangeFeet { get; init; }

    [JsonRequired]
    public string? SavingThrowAbilityId { get; init; }

    [JsonRequired]
    public bool RedirectsAttackToClosestOtherCreature { get; init; }
}
