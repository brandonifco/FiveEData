using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;

namespace FiveEData.Rules.Equipment.Weapons.Serialization;

internal sealed class WeaponDefinitionData
{
    [JsonRequired]
    public string? Id { get; init; }

    [JsonRequired]
    public string? Name { get; init; }

    [JsonRequired]
    public WeaponProficiencyCategory ProficiencyCategory { get; init; }

    [JsonRequired]
    public WeaponUsageCategory UsageCategory { get; init; }

    [JsonRequired]
    public MoneyData? Cost { get; init; }

    [JsonRequired]
    public WeightData? Weight { get; init; }

    [JsonRequired]
    public WeaponDamageData? Damage { get; init; }

    [JsonRequired]
    public WeaponProperty[]? Properties { get; init; }

    [JsonRequired]
    public WeaponRangeData? Range { get; init; }

    [JsonRequired]
    public DiceExpressionData? VersatileDamage { get; init; }

    [JsonRequired]
    public string? AmmunitionTypeId { get; init; }

    [JsonRequired]
    public string[]? SpecialRuleIds { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}

internal sealed class WeaponDamageData
{
    [JsonRequired]
    public DiceExpressionData? Dice { get; init; }

    [JsonRequired]
    public int FixedAmount { get; init; }

    [JsonRequired]
    public string? Type { get; init; }
}

internal sealed class WeaponRangeData
{
    [JsonRequired]
    public int NormalFeet { get; init; }

    [JsonRequired]
    public int LongFeet { get; init; }
}
