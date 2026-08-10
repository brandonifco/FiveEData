using System.Text.Json.Serialization;
using FiveEData.Rules.Classes.Auras.Serialization;
using FiveEData.Rules.Classes.BendLuck.Serialization;
using FiveEData.Rules.Classes.AlterMemories.Serialization;
using FiveEData.Rules.Classes.Assassinate.Serialization;
using FiveEData.Rules.Classes.AwakenedMind.Serialization;
using FiveEData.Rules.Classes.BeguilingDefenses.Serialization;
using FiveEData.Rules.Classes.CircleForms.Serialization;
using FiveEData.Rules.Classes.CreateThrall.Serialization;
using FiveEData.Rules.Classes.DarkDelirium.Serialization;
using FiveEData.Rules.Classes.DraconicPresence.Serialization;
using FiveEData.Rules.Classes.DragonWings.Serialization;
using FiveEData.Rules.Classes.DeathStrike.Serialization;
using FiveEData.Rules.Classes.ElementalAffinity.Serialization;
using FiveEData.Rules.Classes.EntropicWard.Serialization;
using FiveEData.Rules.Classes.FeyPresence.Serialization;
using FiveEData.Rules.Classes.Frenzy.Serialization;
using FiveEData.Rules.Classes.HypnoticGaze.Serialization;
using FiveEData.Rules.Classes.InfiltrationExpertise.Serialization;
using FiveEData.Rules.Classes.InstinctiveCharm.Serialization;
using FiveEData.Rules.Classes.IntimidatingPresence.Serialization;
using FiveEData.Rules.Classes.MistyEscape.Serialization;
using FiveEData.Rules.Classes.Overchannel.Serialization;
using FiveEData.Rules.Classes.PotentCantrip.Serialization;
using FiveEData.Rules.Classes.SculptSpells.Serialization;
using FiveEData.Rules.Classes.SecondStoryWork.Serialization;
using FiveEData.Rules.Classes.ThoughtShield.Serialization;
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
    public FrenzyDetailData? Frenzy { get; init; }

    [JsonRequired]
    public string[]? MindlessRageImmuneConditionIds { get; init; }

    [JsonRequired]
    public IntimidatingPresenceDetailData? IntimidatingPresence
    {
        get;
        init;
    }

    [JsonRequired]
    public SecondStoryWorkDetailData? SecondStoryWork { get; init; }

    [JsonRequired]
    public AssassinateDetailData? Assassinate { get; init; }

    [JsonRequired]
    public InfiltrationExpertiseDetailData? InfiltrationExpertise
    {
        get;
        init;
    }

    [JsonRequired]
    public int? ImpostorRequiredStudyHours { get; init; }

    [JsonRequired]
    public DeathStrikeDetailData? DeathStrike { get; init; }

    [JsonRequired]
    public FeyPresenceDetailData? FeyPresence { get; init; }

    [JsonRequired]
    public MistyEscapeDetailData? MistyEscape { get; init; }

    [JsonRequired]
    public BeguilingDefensesDetailData? BeguilingDefenses { get; init; }

    [JsonRequired]
    public DarkDeliriumDetailData? DarkDelirium { get; init; }

    [JsonRequired]
    public AwakenedMindDetailData? AwakenedMind { get; init; }

    [JsonRequired]
    public EntropicWardDetailData? EntropicWard { get; init; }

    [JsonRequired]
    public ThoughtShieldDetailData? ThoughtShield { get; init; }

    [JsonRequired]
    public CreateThrallDetailData? CreateThrall { get; init; }

    [JsonRequired]
    public HypnoticGazeDetailData? HypnoticGaze { get; init; }

    [JsonRequired]
    public InstinctiveCharmDetailData? InstinctiveCharm { get; init; }

    [JsonRequired]
    public bool? SplitEnchantmentTargetsSecondCreature { get; init; }

    [JsonRequired]
    public AlterMemoriesDetailData? AlterMemories { get; init; }

    [JsonRequired]
    public SculptSpellsDetailData? SculptSpells { get; init; }

    [JsonRequired]
    public PotentCantripDetailData? PotentCantrip { get; init; }

    [JsonRequired]
    public bool? EmpoweredEvocationAddsSpellcastingModifierToDamage
    {
        get;
        init;
    }

    [JsonRequired]
    public OverchannelDetailData? Overchannel { get; init; }

    [JsonRequired]
    public ElementalAffinityDetailData? ElementalAffinity { get; init; }

    [JsonRequired]
    public DragonWingsDetailData? DragonWings { get; init; }

    [JsonRequired]
    public DraconicPresenceDetailData? DraconicPresence { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}
