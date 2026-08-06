using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Skills;

namespace FiveEData.Rules.Backgrounds;

public sealed class BackgroundDefinition
{
    internal BackgroundDefinition(
        BackgroundId id,
        string name,
        IEnumerable<SkillId> skillProficiencyIds,
        int languageChoiceCount,
        RuleId featureRuleId,
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
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public BackgroundId Id { get; }
    public string Name { get; }
    public IReadOnlyList<SkillId> SkillProficiencyIds { get; }
    public int LanguageChoiceCount { get; }
    public RuleId FeatureRuleId { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
