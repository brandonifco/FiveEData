using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;

namespace FiveEData.Rules.Classes.FightingStyles.Serialization;

internal sealed class FightingStyleDefinitionData
{
    [JsonRequired]
    public string? Id { get; init; }

    [JsonRequired]
    public string? Name { get; init; }

    [JsonRequired]
    public string[]? AvailableToClassIds { get; init; }

    [JsonRequired]
    public FightingStyleRollBonusData? RollBonus { get; init; }

    [JsonRequired]
    public int? ArmorClassBonus { get; init; }

    [JsonRequired]
    public FightingStyleDamageDieRerollData? DamageDieReroll { get; init; }

    [JsonRequired]
    public FightingStyleReactionData? Reaction { get; init; }

    [JsonRequired]
    public bool GrantsOffHandAbilityModifierDamage { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}

internal sealed class FightingStyleRollBonusData
{
    [JsonRequired]
    public FightingStyleRollTarget Target { get; init; }

    [JsonRequired]
    public int Amount { get; init; }

    [JsonRequired]
    public FightingStyleWeaponRequirement WeaponRequirement { get; init; }
}

internal sealed class FightingStyleDamageDieRerollData
{
    [JsonRequired]
    public int RerollAtOrBelowValue { get; init; }

    [JsonRequired]
    public FightingStyleWeaponRequirement WeaponRequirement { get; init; }
}

internal sealed class FightingStyleReactionData
{
    [JsonRequired]
    public int RangeFeet { get; init; }

    [JsonRequired]
    public bool RequiresShield { get; init; }
}
