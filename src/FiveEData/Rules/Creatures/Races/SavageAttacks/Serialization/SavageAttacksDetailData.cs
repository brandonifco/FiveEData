using System.Text.Json.Serialization;

namespace FiveEData.Rules.Creatures.Races.SavageAttacks.Serialization;

internal sealed class SavageAttacksDetailData
{
    [JsonRequired]
    public int AdditionalCriticalDice { get; init; }

    [JsonRequired]
    public bool RequiresMeleeWeapon { get; init; }
}
