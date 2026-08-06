using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Skills;
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
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(skillProficiencyIds);
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
    public IReadOnlyList<SourceReference> Sources { get; }
}
