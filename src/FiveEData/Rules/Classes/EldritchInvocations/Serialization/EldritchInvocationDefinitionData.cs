using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;

namespace FiveEData.Rules.Classes.EldritchInvocations.Serialization;

internal sealed class EldritchInvocationDefinitionData
{
    [JsonRequired]
    public string? Id { get; init; }

    [JsonRequired]
    public string? Name { get; init; }

    [JsonRequired]
    public bool RequiresEldritchBlastCantrip { get; init; }

    [JsonRequired]
    public int? RequiredMinimumLevel { get; init; }

    [JsonRequired]
    public WarlockPactBoon? RequiresPactBoon { get; init; }

    [JsonRequired]
    public string? GrantedSpellId { get; init; }

    [JsonRequired]
    public EldritchInvocationCastingFrequency? CastingFrequency
    { get; init; }

    [JsonRequired]
    public bool WaivesMaterialComponents { get; init; }

    [JsonRequired]
    public bool AddsSpellcastingModifierToDamage { get; init; }

    [JsonRequired]
    public string? ExtraDamageTypeId { get; init; }

    [JsonRequired]
    public string[]? SkillProficiencyIds { get; init; }

    [JsonRequired]
    public int? DarknessVisionRangeFeet { get; init; }

    [JsonRequired]
    public int? TrueSightRangeFeet { get; init; }

    [JsonRequired]
    public int? EldritchBlastRangeFeet { get; init; }

    [JsonRequired]
    public int? EldritchBlastPushDistanceFeet { get; init; }

    [JsonRequired]
    public bool CanReadAllWriting { get; init; }

    [JsonRequired]
    public bool GrantsSecondPactWeaponAttack { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}
