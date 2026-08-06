using System.Text.Json.Serialization;
using FiveEData.Rules.Classes.Auras.Serialization;
using FiveEData.Rules.Classes.CircleForms.Serialization;
using FiveEData.Rules.Classes.CombatSuperiority.Serialization;
using FiveEData.Rules.Classes.DivineStrike.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;

namespace FiveEData.Rules.Classes.Serialization;

internal sealed class SubclassDefinitionData
{
    [JsonRequired]
    public string? Id { get; init; }

    [JsonRequired]
    public string? Name { get; init; }

    [JsonRequired]
    public string? ClassId { get; init; }

    [JsonRequired]
    public int ChosenAtLevel { get; init; }

    [JsonRequired]
    public ClassLevelFeatureData[]? LevelFeatures { get; init; }

    [JsonRequired]
    public string? SpellSlotProgressionId { get; init; }

    [JsonRequired]
    public string? SpellcastingAbilityId { get; init; }

    [JsonRequired]
    public DivineStrikeProgressionDetailData? DivineStrikeProgression
    {
        get;
        init;
    }

    [JsonRequired]
    public CircleFormsProgressionDetailData? CircleFormsProgression
    {
        get;
        init;
    }

    [JsonRequired]
    public AuraOfDevotionDetailData? AuraOfDevotion { get; init; }

    [JsonRequired]
    public AuraOfWardingDetailData? AuraOfWarding { get; init; }

    [JsonRequired]
    public CombatSuperiorityProgressionDetailData? CombatSuperiorityProgression
    {
        get;
        init;
    }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}
