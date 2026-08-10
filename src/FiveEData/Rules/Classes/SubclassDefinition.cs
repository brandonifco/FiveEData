using FiveEData.Rules.Classes.Assassinate;
using FiveEData.Rules.Classes.Auras;
using FiveEData.Rules.Classes.BendLuck;
using FiveEData.Rules.Classes.CircleForms;
using FiveEData.Rules.Classes.CombatSuperiority;
using FiveEData.Rules.Classes.DeathStrike;
using FiveEData.Rules.Classes.DiscipleOfTheElements;
using FiveEData.Rules.Classes.DraconicResilience;
using FiveEData.Rules.Classes.Frenzy;
using FiveEData.Rules.Classes.HurlThroughHell;
using FiveEData.Rules.Classes.InfiltrationExpertise;
using FiveEData.Rules.Classes.IntimidatingPresence;
using FiveEData.Rules.Classes.ImprovedCritical;
using FiveEData.Rules.Classes.DivineStrike;
using FiveEData.Rules.Classes.MagicalSecrets;
using FiveEData.Rules.Classes.Portent;
using FiveEData.Rules.Classes.SecondStoryWork;
using FiveEData.Rules.Classes.ShadowStep;
using FiveEData.Rules.Classes.Spellcasting;
using FiveEData.Rules.Classes.ThunderboltStrike;
using FiveEData.Rules.Classes.WardingFlare;
using FiveEData.Rules.Classes.WrathOfTheStorm;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Conditions;

namespace FiveEData.Rules.Classes;

public sealed class SubclassDefinition
{
    internal SubclassDefinition(
        SubclassId id,
        string name,
        ClassId classId,
        int chosenAtLevel,
        IEnumerable<ClassLevelFeature> levelFeatures,
        SpellSlotProgressionId? spellSlotProgressionId,
        AbilityId? spellcastingAbilityId,
        DivineStrikeProgressionDetail? divineStrikeProgression,
        CircleFormsProgressionDetail? circleFormsProgression,
        AuraOfDevotionDetail? auraOfDevotion,
        AuraOfWardingDetail? auraOfWarding,
        CombatSuperiorityProgressionDetail? combatSuperiorityProgression,
        DiscipleOfTheElementsProgressionDetail?
            discipleOfTheElementsProgression,
        MagicalSecretsProgressionDetail? magicalSecretsProgression,
        PortentProgressionDetail? portentProgression,
        DraconicResilienceDetail? draconicResilience,
        ImprovedCriticalProgressionDetail? improvedCriticalProgression,
        ShadowStepDetail? shadowStep,
        HurlThroughHellDetail? hurlThroughHell,
        WrathOfTheStormDetail? wrathOfTheStorm,
        ThunderboltStrikeDetail? thunderboltStrike,
        int? shadowArtsKiCost,
        int? quiveringPalmKiCost,
        int? draconicPresenceSorceryPointCost,
        BendLuckDetail? bendLuck,
        WardingFlareDetail? wardingFlare,
        AbilityModifierUsesGrant? warPriestUsesPerRest,
        IEnumerable<SpellGrant> innateSpellGrants,
        FrenzyDetail? frenzy,
        IEnumerable<ConditionId> mindlessRageImmuneConditionIds,
        IntimidatingPresenceDetail? intimidatingPresence,
        SecondStoryWorkDetail? secondStoryWork,
        AssassinateDetail? assassinate,
        InfiltrationExpertiseDetail? infiltrationExpertise,
        int? impostorRequiredStudyHours,
        DeathStrikeDetail? deathStrike,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(levelFeatures);
        ArgumentNullException.ThrowIfNull(innateSpellGrants);
        ArgumentNullException.ThrowIfNull(mindlessRageImmuneConditionIds);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        ClassId = classId;
        ChosenAtLevel = chosenAtLevel;
        LevelFeatures = Array.AsReadOnly(levelFeatures.ToArray());
        SpellSlotProgressionId = spellSlotProgressionId;
        SpellcastingAbilityId = spellcastingAbilityId;
        DivineStrikeProgression = divineStrikeProgression;
        CircleFormsProgression = circleFormsProgression;
        AuraOfDevotion = auraOfDevotion;
        AuraOfWarding = auraOfWarding;
        CombatSuperiorityProgression = combatSuperiorityProgression;
        DiscipleOfTheElementsProgression = discipleOfTheElementsProgression;
        MagicalSecretsProgression = magicalSecretsProgression;
        PortentProgression = portentProgression;
        DraconicResilience = draconicResilience;
        ImprovedCriticalProgression = improvedCriticalProgression;
        ShadowStep = shadowStep;
        HurlThroughHell = hurlThroughHell;
        WrathOfTheStorm = wrathOfTheStorm;
        ThunderboltStrike = thunderboltStrike;
        ShadowArtsKiCost = shadowArtsKiCost;
        QuiveringPalmKiCost = quiveringPalmKiCost;
        DraconicPresenceSorceryPointCost =
            draconicPresenceSorceryPointCost;
        BendLuck = bendLuck;
        WardingFlare = wardingFlare;
        WarPriestUsesPerRest = warPriestUsesPerRest;
        InnateSpellGrants = Array.AsReadOnly(innateSpellGrants.ToArray());
        Frenzy = frenzy;
        MindlessRageImmuneConditionIds =
            Array.AsReadOnly(mindlessRageImmuneConditionIds.ToArray());
        IntimidatingPresence = intimidatingPresence;
        SecondStoryWork = secondStoryWork;
        Assassinate = assassinate;
        InfiltrationExpertise = infiltrationExpertise;
        ImpostorRequiredStudyHours = impostorRequiredStudyHours;
        DeathStrike = deathStrike;
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public SubclassId Id { get; }
    public string Name { get; }
    public ClassId ClassId { get; }
    public int ChosenAtLevel { get; }
    public IReadOnlyList<ClassLevelFeature> LevelFeatures { get; }
    public SpellSlotProgressionId? SpellSlotProgressionId { get; }
    public AbilityId? SpellcastingAbilityId { get; }
    public DivineStrikeProgressionDetail? DivineStrikeProgression { get; }
    public CircleFormsProgressionDetail? CircleFormsProgression { get; }
    public AuraOfDevotionDetail? AuraOfDevotion { get; }
    public AuraOfWardingDetail? AuraOfWarding { get; }

    public CombatSuperiorityProgressionDetail? CombatSuperiorityProgression
    { get; }

    public DiscipleOfTheElementsProgressionDetail?
        DiscipleOfTheElementsProgression
    { get; }

    public MagicalSecretsProgressionDetail? MagicalSecretsProgression
    {
        get;
    }

    public PortentProgressionDetail? PortentProgression { get; }

    public DraconicResilienceDetail? DraconicResilience { get; }

    public ImprovedCriticalProgressionDetail? ImprovedCriticalProgression
    {
        get;
    }

    public ShadowStepDetail? ShadowStep { get; }

    public HurlThroughHellDetail? HurlThroughHell { get; }

    public WrathOfTheStormDetail? WrathOfTheStorm { get; }

    public ThunderboltStrikeDetail? ThunderboltStrike { get; }

    public int? ShadowArtsKiCost { get; }

    public int? QuiveringPalmKiCost { get; }

    public int? DraconicPresenceSorceryPointCost { get; }

    public BendLuckDetail? BendLuck { get; }

    public WardingFlareDetail? WardingFlare { get; }

    public AbilityModifierUsesGrant? WarPriestUsesPerRest { get; }

    public IReadOnlyList<SpellGrant> InnateSpellGrants { get; }

    public FrenzyDetail? Frenzy { get; }

    public IReadOnlyList<ConditionId> MindlessRageImmuneConditionIds
    { get; }

    public IntimidatingPresenceDetail? IntimidatingPresence { get; }

    public SecondStoryWorkDetail? SecondStoryWork { get; }

    public AssassinateDetail? Assassinate { get; }

    public InfiltrationExpertiseDetail? InfiltrationExpertise { get; }

    public int? ImpostorRequiredStudyHours { get; }

    public DeathStrikeDetail? DeathStrike { get; }

    public IReadOnlyList<SourceReference> Sources { get; }
}
