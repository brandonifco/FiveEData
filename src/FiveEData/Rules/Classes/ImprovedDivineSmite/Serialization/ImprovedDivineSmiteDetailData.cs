using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Classes.ImprovedDivineSmite.Serialization;

internal sealed class ImprovedDivineSmiteDetailData
{
    [JsonRequired]
    public DiceExpressionData? Damage { get; init; }

    [JsonRequired]
    public string? DamageTypeId { get; init; }

    [JsonRequired]
    public bool RequiresMeleeWeapon { get; init; }
}
