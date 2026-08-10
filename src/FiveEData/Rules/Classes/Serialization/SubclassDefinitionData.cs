using System.Text.Json.Serialization;
using FiveEData.Rules.Classes.Auras.Serialization;
using FiveEData.Rules.Classes.BendLuck.Serialization;
using FiveEData.Rules.Classes.CircleForms.Serialization;
using FiveEData.Rules.Classes.CombatSuperiority.Serialization;
using FiveEData.Rules.Classes.DiscipleOfTheElements.Serialization;
using FiveEData.Rules.Classes.DraconicResilience.Serialization;
using FiveEData.Rules.Classes.HurlThroughHell.Serialization;
using FiveEData.Rules.Classes.ImprovedCritical.Serialization;
using FiveEData.Rules.Classes.MagicalSecrets.Serialization;
using FiveEData.Rules.Classes.Portent.Serialization;
using FiveEData.Rules.Classes.ShadowStep.Serialization;
using FiveEData.Rules.Classes.ThunderboltStrike.Serialization;
using FiveEData.Rules.Classes.WardingFlare.Serialization;
using FiveEData.Rules.Classes.WrathOfTheStorm.Serialization;
using FiveEData.Rules.Classes.DivineStrike.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;

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
    public DiscipleOfTheElementsProgressionDetailData?
        DiscipleOfTheElementsProgression
    {
        get;
        init;
    }

    [JsonRequired]
    public MagicalSecretsProgressionDetailData? MagicalSecretsProgression
    {
        get;
        init;
    }

    [JsonRequired]
    public PortentProgressionDetailData? PortentProgression { get; init; }

    [JsonRequired]
    public DraconicResilienceDetailData? DraconicResilience { get; init; }

    [JsonRequired]
    public ImprovedCriticalProgressionDetailData?
        ImprovedCriticalProgression
    {
        get;
        init;
    }

    [JsonRequired]
    public ShadowStepDetailData? ShadowStep { get; init; }

    [JsonRequired]
    public HurlThroughHellDetailData? HurlThroughHell { get; init; }

    [JsonRequired]
    public WrathOfTheStormDetailData? WrathOfTheStorm { get; init; }

    [JsonRequired]
    public ThunderboltStrikeDetailData? ThunderboltStrike { get; init; }

    [JsonRequired]
    public int? ShadowArtsKiCost { get; init; }

    [JsonRequired]
    public int? QuiveringPalmKiCost { get; init; }

    [JsonRequired]
    public int? DraconicPresenceSorceryPointCost { get; init; }

    [JsonRequired]
    public BendLuckDetailData? BendLuck { get; init; }

    [JsonRequired]
    public WardingFlareDetailData? WardingFlare { get; init; }

    [JsonRequired]
    public AbilityModifierUsesGrantData? WarPriestUsesPerRest { get; init; }

    [JsonRequired]
    public SpellGrantData[]? InnateSpellGrants { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}
