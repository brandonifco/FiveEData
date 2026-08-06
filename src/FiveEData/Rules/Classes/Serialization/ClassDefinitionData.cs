using System.Text.Json.Serialization;
using FiveEData.Rules.Classes.Auras.Serialization;
using FiveEData.Rules.Classes.BardicInspiration.Serialization;
using FiveEData.Rules.Classes.ChannelDivinity.Serialization;
using FiveEData.Rules.Classes.EldritchInvocationsKnown.Serialization;
using FiveEData.Rules.Classes.FontOfMagic.Serialization;
using FiveEData.Rules.Classes.Ki.Serialization;
using FiveEData.Rules.Classes.MysticArcanum.Serialization;
using FiveEData.Rules.Classes.Rage.Serialization;
using FiveEData.Rules.Classes.SneakAttack.Serialization;
using FiveEData.Rules.Classes.SongOfRest.Serialization;
using FiveEData.Rules.Classes.SorceryPoints.Serialization;
using FiveEData.Rules.Classes.WildShape.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Equipment.Armor;
using FiveEData.Rules.Equipment.Weapons;

namespace FiveEData.Rules.Classes.Serialization;

internal sealed class ClassDefinitionData
{
    [JsonRequired]
    public string? Id { get; init; }

    [JsonRequired]
    public string? Name { get; init; }

    [JsonRequired]
    public int HitDieSides { get; init; }

    [JsonRequired]
    public string[]? PrimaryAbilityIds { get; init; }

    [JsonRequired]
    public bool RequiresAllPrimaryAbilities { get; init; }

    [JsonRequired]
    public string[]? SavingThrowProficiencyIds { get; init; }

    [JsonRequired]
    public ArmorCategory[]? ArmorProficiencyCategories { get; init; }

    [JsonRequired]
    public bool ProficientWithShields { get; init; }

    [JsonRequired]
    public WeaponProficiencyCategory[]? WeaponProficiencyCategories { get; init; }

    [JsonRequired]
    public string[]? WeaponProficiencyIds { get; init; }

    [JsonRequired]
    public int SkillChoiceCount { get; init; }

    [JsonRequired]
    public string[]? SkillChoiceOptionIds { get; init; }

    [JsonRequired]
    public ClassLevelFeatureData[]? LevelFeatures { get; init; }

    [JsonRequired]
    public string? SpellSlotProgressionId { get; init; }

    [JsonRequired]
    public string? SpellcastingAbilityId { get; init; }

    [JsonRequired]
    public string? ExtraAttackProgressionId { get; init; }

    [JsonRequired]
    public RageProgressionDetailData? RageProgression { get; init; }

    [JsonRequired]
    public SneakAttackProgressionDetailData? SneakAttackProgression
    {
        get;
        init;
    }

    [JsonRequired]
    public KiProgressionDetailData? KiProgression { get; init; }

    [JsonRequired]
    public SorceryPointsProgressionDetailData? SorceryPointsProgression
    {
        get;
        init;
    }

    [JsonRequired]
    public WildShapeProgressionDetailData? WildShapeProgression
    {
        get;
        init;
    }

    [JsonRequired]
    public AuraOfProtectionDetailData? AuraOfProtection { get; init; }

    [JsonRequired]
    public AuraOfCourageDetailData? AuraOfCourage { get; init; }

    [JsonRequired]
    public BardicInspirationProgressionDetailData? BardicInspirationProgression
    {
        get;
        init;
    }

    [JsonRequired]
    public ChannelDivinityProgressionDetailData? ChannelDivinityProgression
    {
        get;
        init;
    }

    [JsonRequired]
    public MysticArcanumProgressionDetailData? MysticArcanumProgression
    {
        get;
        init;
    }

    [JsonRequired]
    public FontOfMagicConversionDetailData? FontOfMagicConversion
    {
        get;
        init;
    }

    [JsonRequired]
    public SongOfRestProgressionDetailData? SongOfRestProgression
    {
        get;
        init;
    }

    [JsonRequired]
    public EldritchInvocationsKnownProgressionDetailData?
        EldritchInvocationsKnownProgression
    {
        get;
        init;
    }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}

internal sealed class ClassLevelFeatureData
{
    [JsonRequired]
    public int Level { get; init; }

    [JsonRequired]
    public string? FeatureRuleId { get; init; }
}
