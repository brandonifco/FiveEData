using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.DamageTypes;

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
        int? darkvisionRangeFeet,
        IEnumerable<DamageTypeId> resistedDamageTypeIds,
        int? hitPointBonusPerLevel,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(abilityScoreIncreases);
        ArgumentNullException.ThrowIfNull(traitRuleIds);
        ArgumentNullException.ThrowIfNull(resistedDamageTypeIds);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        RaceId = raceId;
        AbilityScoreIncreases =
            Array.AsReadOnly(abilityScoreIncreases.ToArray());
        Speed = speed;
        AdditionalLanguageChoiceCount = additionalLanguageChoiceCount;
        TraitRuleIds = Array.AsReadOnly(traitRuleIds.ToArray());
        DarkvisionRangeFeet = darkvisionRangeFeet;
        ResistedDamageTypeIds =
            Array.AsReadOnly(resistedDamageTypeIds.ToArray());
        HitPointBonusPerLevel = hitPointBonusPerLevel;
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public SubraceId Id { get; }
    public string Name { get; }
    public RaceId RaceId { get; }
    public IReadOnlyList<RaceAbilityScoreIncrease> AbilityScoreIncreases { get; }
    public Distance? Speed { get; }
    public int AdditionalLanguageChoiceCount { get; }
    public IReadOnlyList<RuleId> TraitRuleIds { get; }
    public int? DarkvisionRangeFeet { get; }
    public IReadOnlyList<DamageTypeId> ResistedDamageTypeIds { get; }
    public int? HitPointBonusPerLevel { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
