using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.DamageTypes;
using FiveEData.Rules.Creatures.Skills;
using FiveEData.Rules.Spells;

namespace FiveEData.Rules.Classes.EldritchInvocations;

public sealed class EldritchInvocationDefinition
{
    internal EldritchInvocationDefinition(
        EldritchInvocationId id,
        string name,
        bool requiresEldritchBlastCantrip,
        int? requiredMinimumLevel,
        WarlockPactBoon? requiresPactBoon,
        SpellId? grantedSpellId,
        EldritchInvocationCastingFrequency? castingFrequency,
        bool waivesMaterialComponents,
        bool addsSpellcastingModifierToDamage,
        DamageTypeId? extraDamageTypeId,
        IEnumerable<SkillId> skillProficiencyIds,
        int? darknessVisionRangeFeet,
        int? trueSightRangeFeet,
        int? eldritchBlastRangeFeet,
        int? eldritchBlastPushDistanceFeet,
        bool canReadAllWriting,
        bool grantsSecondPactWeaponAttack,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(skillProficiencyIds);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        RequiresEldritchBlastCantrip = requiresEldritchBlastCantrip;
        RequiredMinimumLevel = requiredMinimumLevel;
        RequiresPactBoon = requiresPactBoon;
        GrantedSpellId = grantedSpellId;
        CastingFrequency = castingFrequency;
        WaivesMaterialComponents = waivesMaterialComponents;
        AddsSpellcastingModifierToDamage = addsSpellcastingModifierToDamage;
        ExtraDamageTypeId = extraDamageTypeId;
        SkillProficiencyIds =
            Array.AsReadOnly(skillProficiencyIds.ToArray());
        DarknessVisionRangeFeet = darknessVisionRangeFeet;
        TrueSightRangeFeet = trueSightRangeFeet;
        EldritchBlastRangeFeet = eldritchBlastRangeFeet;
        EldritchBlastPushDistanceFeet = eldritchBlastPushDistanceFeet;
        CanReadAllWriting = canReadAllWriting;
        GrantsSecondPactWeaponAttack = grantsSecondPactWeaponAttack;
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public EldritchInvocationId Id { get; }
    public string Name { get; }
    public bool RequiresEldritchBlastCantrip { get; }
    public int? RequiredMinimumLevel { get; }
    public WarlockPactBoon? RequiresPactBoon { get; }
    public SpellId? GrantedSpellId { get; }
    public EldritchInvocationCastingFrequency? CastingFrequency { get; }
    public bool WaivesMaterialComponents { get; }
    public bool AddsSpellcastingModifierToDamage { get; }
    public DamageTypeId? ExtraDamageTypeId { get; }
    public IReadOnlyList<SkillId> SkillProficiencyIds { get; }
    public int? DarknessVisionRangeFeet { get; }
    public int? TrueSightRangeFeet { get; }
    public int? EldritchBlastRangeFeet { get; }
    public int? EldritchBlastPushDistanceFeet { get; }
    public bool CanReadAllWriting { get; }
    public bool GrantsSecondPactWeaponAttack { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
