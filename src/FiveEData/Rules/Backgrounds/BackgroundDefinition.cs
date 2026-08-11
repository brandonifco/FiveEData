using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Skills;
using FiveEData.Rules.Equipment.Tools;
using FiveEData.Rules.Equipment.Vehicles;
using FiveEData.Rules.Expenses.Lifestyles;

namespace FiveEData.Rules.Backgrounds;

public sealed class BackgroundDefinition
{
    internal BackgroundDefinition(
        BackgroundId id,
        string name,
        IEnumerable<SkillId> skillProficiencyIds,
        int languageChoiceCount,
        RuleId featureRuleId,
        LifestyleId? sustainedLifestyleId,
        int? additionalPeopleFedPerDay,
        int? guildDuesGoldPerMonth,
        int? fastTravelSpeedMultiplier,
        IEnumerable<ToolId> toolProficiencyIds,
        ToolProficiencyChoice? toolProficiencyChoice,
        IEnumerable<VehicleKind> vehicleProficiencyKinds,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(skillProficiencyIds);
        ArgumentNullException.ThrowIfNull(toolProficiencyIds);
        ArgumentNullException.ThrowIfNull(vehicleProficiencyKinds);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        SkillProficiencyIds =
            Array.AsReadOnly(skillProficiencyIds.ToArray());
        LanguageChoiceCount = languageChoiceCount;
        FeatureRuleId = featureRuleId;
        SustainedLifestyleId = sustainedLifestyleId;
        AdditionalPeopleFedPerDay = additionalPeopleFedPerDay;
        GuildDuesGoldPerMonth = guildDuesGoldPerMonth;
        FastTravelSpeedMultiplier = fastTravelSpeedMultiplier;
        ToolProficiencyIds = Array.AsReadOnly(toolProficiencyIds.ToArray());
        ToolProficiencyChoice = toolProficiencyChoice;
        VehicleProficiencyKinds =
            Array.AsReadOnly(vehicleProficiencyKinds.ToArray());
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public BackgroundId Id { get; }
    public string Name { get; }
    public IReadOnlyList<SkillId> SkillProficiencyIds { get; }
    public int LanguageChoiceCount { get; }
    public RuleId FeatureRuleId { get; }
    public LifestyleId? SustainedLifestyleId { get; }
    public int? AdditionalPeopleFedPerDay { get; }
    public int? GuildDuesGoldPerMonth { get; }
    public int? FastTravelSpeedMultiplier { get; }
    public IReadOnlyList<ToolId> ToolProficiencyIds { get; }

    public ToolProficiencyChoice? ToolProficiencyChoice { get; }

    /// <summary>
    /// "vehicles (land)" / "vehicles (water)" — the PHB Tools table
    /// prints one combined pointer row for these, so they reuse the
    /// Land/Water axis <see cref="VehicleDefinition"/> already models
    /// rather than becoming tool entries.
    /// </summary>
    public IReadOnlyList<VehicleKind> VehicleProficiencyKinds { get; }

    public IReadOnlyList<SourceReference> Sources { get; }
}
