using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Spells.Serialization;

internal sealed class SpellDefinitionData
{
    [JsonRequired]
    public string? Id { get; init; }

    [JsonRequired]
    public string? Name { get; init; }

    [JsonRequired]
    public int? Level { get; init; }

    [JsonRequired]
    public string? SchoolId { get; init; }

    [JsonRequired]
    public SpellCastingTimeData? CastingTime { get; init; }

    [JsonRequired]
    public SpellRangeData? Range { get; init; }

    [JsonRequired]
    public SpellComponentsData? Components { get; init; }

    [JsonRequired]
    public SpellDurationData? Duration { get; init; }

    [JsonRequired]
    public bool? IsRitual { get; init; }

    [JsonRequired]
    public SpellDamageEffectData? DamageEffect { get; init; }

    [JsonRequired]
    public string[]? AvailableToClassIds { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}

internal sealed class SpellDamageEffectData
{
    [JsonRequired]
    public string? DamageTypeId { get; init; }

    [JsonRequired]
    public string? AttackRollType { get; init; }

    [JsonRequired]
    public string? SavingThrowAbilityId { get; init; }

    [JsonRequired]
    public SpellDamageTierGrantData[]? DamageByCharacterLevel { get; init; }
}

internal sealed class SpellDamageTierGrantData
{
    [JsonRequired]
    public int? CharacterLevel { get; init; }

    [JsonRequired]
    public DiceExpressionData? Damage { get; init; }
}

internal sealed class SpellCastingTimeData
{
    [JsonRequired]
    public int? Amount { get; init; }

    [JsonRequired]
    public string? Unit { get; init; }
}

internal sealed class SpellRangeData
{
    [JsonRequired]
    public string? Kind { get; init; }

    [JsonRequired]
    public int? DistanceFeet { get; init; }

    [JsonRequired]
    public string? AreaShape { get; init; }

    [JsonRequired]
    public int? AreaSizeFeet { get; init; }
}

internal sealed class SpellComponentsData
{
    [JsonRequired]
    public bool? Verbal { get; init; }

    [JsonRequired]
    public bool? Somatic { get; init; }

    [JsonRequired]
    public bool? Material { get; init; }

    [JsonRequired]
    public string? MaterialDescription { get; init; }

    [JsonRequired]
    public int? MaterialCostGoldPieces { get; init; }

    [JsonRequired]
    public bool? MaterialIsConsumed { get; init; }
}

internal sealed class SpellDurationData
{
    [JsonRequired]
    public bool? IsInstantaneous { get; init; }

    [JsonRequired]
    public bool? IsUntilDispelled { get; init; }

    [JsonRequired]
    public bool? IsSpecial { get; init; }

    [JsonRequired]
    public bool? RequiresConcentration { get; init; }

    [JsonRequired]
    public bool? IsUpTo { get; init; }

    [JsonRequired]
    public int? Amount { get; init; }

    [JsonRequired]
    public string? Unit { get; init; }
}
