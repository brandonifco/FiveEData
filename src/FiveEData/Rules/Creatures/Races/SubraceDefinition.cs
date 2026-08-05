using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Creatures.Races;

public sealed class SubraceDefinition
{
    internal SubraceDefinition(
        SubraceId id,
        string name,
        RaceId raceId,
        IEnumerable<RaceAbilityScoreIncrease> abilityScoreIncreases,
        Distance? speed,
        int additionalLanguageChoiceCount,
        IEnumerable<RuleId> traitRuleIds,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(abilityScoreIncreases);
        ArgumentNullException.ThrowIfNull(traitRuleIds);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        RaceId = raceId;
        AbilityScoreIncreases =
            Array.AsReadOnly(abilityScoreIncreases.ToArray());
        Speed = speed;
        AdditionalLanguageChoiceCount = additionalLanguageChoiceCount;
        TraitRuleIds = Array.AsReadOnly(traitRuleIds.ToArray());
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public SubraceId Id { get; }
    public string Name { get; }
    public RaceId RaceId { get; }
    public IReadOnlyList<RaceAbilityScoreIncrease> AbilityScoreIncreases { get; }
    public Distance? Speed { get; }
    public int AdditionalLanguageChoiceCount { get; }
    public IReadOnlyList<RuleId> TraitRuleIds { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
