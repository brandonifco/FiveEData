using FiveEData.Rules.Classes.ActionSurge;
using FiveEData.Rules.Classes.Auras;
using FiveEData.Rules.Classes.BardicInspiration;
using FiveEData.Rules.Classes.ChannelDivinity;
using FiveEData.Rules.Classes.BrutalCritical;
using FiveEData.Rules.Classes.DestroyUndead;
using FiveEData.Rules.Classes.EldritchInvocationsKnown;
using FiveEData.Rules.Classes.ExtraAttack;
using FiveEData.Rules.Classes.FastMovement;
using FiveEData.Rules.Classes.FavoredEnemy;
using FiveEData.Rules.Classes.FontOfMagic;
using FiveEData.Rules.Classes.Indomitable;
using FiveEData.Rules.Classes.Ki;
using FiveEData.Rules.Classes.MagicalSecrets;
using FiveEData.Rules.Classes.MartialArts;
using FiveEData.Rules.Classes.MysticArcanum;
using FiveEData.Rules.Classes.NaturalExplorer;
using FiveEData.Rules.Classes.Rage;
using FiveEData.Rules.Classes.SneakAttack;
using FiveEData.Rules.Classes.SongOfRest;
using FiveEData.Rules.Classes.SorceryPoints;
using FiveEData.Rules.Classes.Spellcasting;
using FiveEData.Rules.Classes.UnarmoredMovement;
using FiveEData.Rules.Classes.WildShape;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Skills;
using FiveEData.Rules.Equipment.Armor;
using FiveEData.Rules.Equipment.Weapons;

namespace FiveEData.Rules.Classes;

public sealed class ClassDefinition
{
    internal ClassDefinition(
        ClassId id,
        string name,
        DiceExpression hitDie,
        IEnumerable<AbilityId> primaryAbilityIds,
        bool requiresAllPrimaryAbilities,
        IEnumerable<AbilityId> savingThrowProficiencyIds,
        IEnumerable<ArmorCategory> armorProficiencyCategories,
        bool proficientWithShields,
        IEnumerable<WeaponProficiencyCategory> weaponProficiencyCategories,
        IEnumerable<WeaponId> weaponProficiencyIds,
        int skillChoiceCount,
        IEnumerable<SkillId> skillChoiceOptionIds,
        IEnumerable<ClassLevelFeature> levelFeatures,
        SpellSlotProgressionId? spellSlotProgressionId,
        AbilityId? spellcastingAbilityId,
        ExtraAttackProgressionId? extraAttackProgressionId,
        ActionSurgeProgressionDetail? actionSurgeProgression,
        IndomitableProgressionDetail? indomitableProgression,
        RageProgressionDetail? rageProgression,
        BrutalCriticalProgressionDetail? brutalCriticalProgression,
        FastMovementDetail? fastMovement,
        FavoredEnemyProgressionDetail? favoredEnemyProgression,
        NaturalExplorerProgressionDetail? naturalExplorerProgression,
        SneakAttackProgressionDetail? sneakAttackProgression,
        KiProgressionDetail? kiProgression,
        MartialArtsProgressionDetail? martialArtsProgression,
        UnarmoredMovementProgressionDetail? unarmoredMovementProgression,
        SorceryPointsProgressionDetail? sorceryPointsProgression,
        WildShapeProgressionDetail? wildShapeProgression,
        AuraOfProtectionDetail? auraOfProtection,
        AuraOfCourageDetail? auraOfCourage,
        BardicInspirationProgressionDetail? bardicInspirationProgression,
        MagicalSecretsProgressionDetail? magicalSecretsProgression,
        ChannelDivinityProgressionDetail? channelDivinityProgression,
        DestroyUndeadProgressionDetail? destroyUndeadProgression,
        MysticArcanumProgressionDetail? mysticArcanumProgression,
        FontOfMagicConversionDetail? fontOfMagicConversion,
        SongOfRestProgressionDetail? songOfRestProgression,
        EldritchInvocationsKnownProgressionDetail?
            eldritchInvocationsKnownProgression,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(primaryAbilityIds);
        ArgumentNullException.ThrowIfNull(savingThrowProficiencyIds);
        ArgumentNullException.ThrowIfNull(armorProficiencyCategories);
        ArgumentNullException.ThrowIfNull(weaponProficiencyCategories);
        ArgumentNullException.ThrowIfNull(weaponProficiencyIds);
        ArgumentNullException.ThrowIfNull(skillChoiceOptionIds);
        ArgumentNullException.ThrowIfNull(levelFeatures);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        HitDie = hitDie;
        PrimaryAbilityIds = Array.AsReadOnly(primaryAbilityIds.ToArray());
        RequiresAllPrimaryAbilities = requiresAllPrimaryAbilities;
        SavingThrowProficiencyIds =
            Array.AsReadOnly(savingThrowProficiencyIds.ToArray());
        ArmorProficiencyCategories =
            Array.AsReadOnly(armorProficiencyCategories.ToArray());
        ProficientWithShields = proficientWithShields;
        WeaponProficiencyCategories =
            Array.AsReadOnly(weaponProficiencyCategories.ToArray());
        WeaponProficiencyIds = Array.AsReadOnly(weaponProficiencyIds.ToArray());
        SkillChoiceCount = skillChoiceCount;
        SkillChoiceOptionIds = Array.AsReadOnly(skillChoiceOptionIds.ToArray());
        LevelFeatures = Array.AsReadOnly(levelFeatures.ToArray());
        SpellSlotProgressionId = spellSlotProgressionId;
        SpellcastingAbilityId = spellcastingAbilityId;
        ExtraAttackProgressionId = extraAttackProgressionId;
        ActionSurgeProgression = actionSurgeProgression;
        IndomitableProgression = indomitableProgression;
        RageProgression = rageProgression;
        BrutalCriticalProgression = brutalCriticalProgression;
        FastMovement = fastMovement;
        FavoredEnemyProgression = favoredEnemyProgression;
        NaturalExplorerProgression = naturalExplorerProgression;
        SneakAttackProgression = sneakAttackProgression;
        KiProgression = kiProgression;
        MartialArtsProgression = martialArtsProgression;
        UnarmoredMovementProgression = unarmoredMovementProgression;
        SorceryPointsProgression = sorceryPointsProgression;
        WildShapeProgression = wildShapeProgression;
        AuraOfProtection = auraOfProtection;
        AuraOfCourage = auraOfCourage;
        BardicInspirationProgression = bardicInspirationProgression;
        MagicalSecretsProgression = magicalSecretsProgression;
        ChannelDivinityProgression = channelDivinityProgression;
        DestroyUndeadProgression = destroyUndeadProgression;
        MysticArcanumProgression = mysticArcanumProgression;
        FontOfMagicConversion = fontOfMagicConversion;
        SongOfRestProgression = songOfRestProgression;
        EldritchInvocationsKnownProgression =
            eldritchInvocationsKnownProgression;
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public ClassId Id { get; }
    public string Name { get; }
    public DiceExpression HitDie { get; }
    public IReadOnlyList<AbilityId> PrimaryAbilityIds { get; }
    public bool RequiresAllPrimaryAbilities { get; }
    public IReadOnlyList<AbilityId> SavingThrowProficiencyIds { get; }
    public IReadOnlyList<ArmorCategory> ArmorProficiencyCategories { get; }
    public bool ProficientWithShields { get; }
    public IReadOnlyList<WeaponProficiencyCategory> WeaponProficiencyCategories { get; }
    public IReadOnlyList<WeaponId> WeaponProficiencyIds { get; }
    public int SkillChoiceCount { get; }
    public IReadOnlyList<SkillId> SkillChoiceOptionIds { get; }
    public IReadOnlyList<ClassLevelFeature> LevelFeatures { get; }
    public SpellSlotProgressionId? SpellSlotProgressionId { get; }
    public AbilityId? SpellcastingAbilityId { get; }
    public ExtraAttackProgressionId? ExtraAttackProgressionId { get; }

    public ActionSurgeProgressionDetail? ActionSurgeProgression { get; }

    public IndomitableProgressionDetail? IndomitableProgression { get; }
    public RageProgressionDetail? RageProgression { get; }

    public BrutalCriticalProgressionDetail? BrutalCriticalProgression
    {
        get;
    }

    public FastMovementDetail? FastMovement { get; }

    public FavoredEnemyProgressionDetail? FavoredEnemyProgression
    {
        get;
    }

    public NaturalExplorerProgressionDetail? NaturalExplorerProgression
    {
        get;
    }
    public SneakAttackProgressionDetail? SneakAttackProgression { get; }
    public KiProgressionDetail? KiProgression { get; }
    public MartialArtsProgressionDetail? MartialArtsProgression { get; }

    public UnarmoredMovementProgressionDetail? UnarmoredMovementProgression
    {
        get;
    }

    public SorceryPointsProgressionDetail? SorceryPointsProgression { get; }
    public WildShapeProgressionDetail? WildShapeProgression { get; }
    public AuraOfProtectionDetail? AuraOfProtection { get; }
    public AuraOfCourageDetail? AuraOfCourage { get; }
    public BardicInspirationProgressionDetail? BardicInspirationProgression
    {
        get;
    }

    public MagicalSecretsProgressionDetail? MagicalSecretsProgression { get; }

    public ChannelDivinityProgressionDetail? ChannelDivinityProgression
    {
        get;
    }

    public DestroyUndeadProgressionDetail? DestroyUndeadProgression { get; }

    public MysticArcanumProgressionDetail? MysticArcanumProgression
    {
        get;
    }
    public FontOfMagicConversionDetail? FontOfMagicConversion { get; }
    public SongOfRestProgressionDetail? SongOfRestProgression { get; }

    public EldritchInvocationsKnownProgressionDetail?
        EldritchInvocationsKnownProgression
    { get; }

    public IReadOnlyList<SourceReference> Sources { get; }
}
