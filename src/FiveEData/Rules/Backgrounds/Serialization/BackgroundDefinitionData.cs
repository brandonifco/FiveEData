using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Equipment.Vehicles;
using FiveEData.Rules.Common.Provenance.Serialization;

namespace FiveEData.Rules.Backgrounds.Serialization;

internal sealed class BackgroundDefinitionData
{
    [JsonRequired]
    public string? Id { get; init; }

    [JsonRequired]
    public string? Name { get; init; }

    [JsonRequired]
    public string[]? SkillProficiencyIds { get; init; }

    [JsonRequired]
    public int LanguageChoiceCount { get; init; }

    [JsonRequired]
    public string? FeatureRuleId { get; init; }

    [JsonRequired]
    public string? SustainedLifestyleId { get; init; }

    [JsonRequired]
    public int? AdditionalPeopleFedPerDay { get; init; }

    [JsonRequired]
    public int? GuildDuesGoldPerMonth { get; init; }

    [JsonRequired]
    public int? FastTravelSpeedMultiplier { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }

    [JsonRequired]
    public string[]? ToolProficiencyIds { get; init; }

    [JsonRequired]
    public ToolProficiencyChoiceData? ToolProficiencyChoice { get; init; }

    [JsonRequired]
    public VehicleKind[]? VehicleProficiencyKinds { get; init; }
}
