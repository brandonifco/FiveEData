using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.DamageTypes;
using FiveEData.Rules.Creatures.Languages;
using FiveEData.Rules.Creatures.Races.BreathWeapon;
using FiveEData.Rules.Creatures.Races.Lucky;
using FiveEData.Rules.Creatures.Races.RelentlessEndurance;
using FiveEData.Rules.Creatures.Races.SavageAttacks;
using FiveEData.Rules.Creatures.Sizes;

namespace FiveEData.Rules.Creatures.Races;

public sealed class RaceDefinition
{
    internal RaceDefinition(
        RaceId id,
        string name,
        CreatureSizeId size,
        Distance speed,
        IEnumerable<RaceAbilityScoreIncrease> abilityScoreIncreases,
        int choosableAbilityScoreIncreaseCount,
        IEnumerable<LanguageId> languageIds,
        int additionalLanguageChoiceCount,
        IEnumerable<RuleId> traitRuleIds,
        int? darkvisionRangeFeet,
        IEnumerable<DamageTypeId> resistedDamageTypeIds,
        int? tranceDurationHours,
        BreathWeaponProgressionDetail? breathWeaponProgression,
        SavageAttacksDetail? savageAttacks,
        RelentlessEnduranceDetail? relentlessEndurance,
        LuckyDetail? lucky,
        IEnumerable<SourceReference> sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(abilityScoreIncreases);
        ArgumentNullException.ThrowIfNull(languageIds);
        ArgumentNullException.ThrowIfNull(traitRuleIds);
        ArgumentNullException.ThrowIfNull(resistedDamageTypeIds);
        ArgumentNullException.ThrowIfNull(sources);

        Id = id;
        Name = name;
        Size = size;
        Speed = speed;
        AbilityScoreIncreases =
            Array.AsReadOnly(abilityScoreIncreases.ToArray());
        ChoosableAbilityScoreIncreaseCount =
            choosableAbilityScoreIncreaseCount;
        LanguageIds = Array.AsReadOnly(languageIds.ToArray());
        AdditionalLanguageChoiceCount = additionalLanguageChoiceCount;
        TraitRuleIds = Array.AsReadOnly(traitRuleIds.ToArray());
        DarkvisionRangeFeet = darkvisionRangeFeet;
        ResistedDamageTypeIds =
            Array.AsReadOnly(resistedDamageTypeIds.ToArray());
        TranceDurationHours = tranceDurationHours;
        BreathWeaponProgression = breathWeaponProgression;
        SavageAttacks = savageAttacks;
        RelentlessEndurance = relentlessEndurance;
        Lucky = lucky;
        Sources = Array.AsReadOnly(sources.ToArray());
    }

    public RaceId Id { get; }
    public string Name { get; }
    public CreatureSizeId Size { get; }
    public Distance Speed { get; }
    public IReadOnlyList<RaceAbilityScoreIncrease> AbilityScoreIncreases { get; }
    public int ChoosableAbilityScoreIncreaseCount { get; }
    public IReadOnlyList<LanguageId> LanguageIds { get; }
    public int AdditionalLanguageChoiceCount { get; }
    public IReadOnlyList<RuleId> TraitRuleIds { get; }
    public int? DarkvisionRangeFeet { get; }
    public IReadOnlyList<DamageTypeId> ResistedDamageTypeIds { get; }
    public int? TranceDurationHours { get; }
    public BreathWeaponProgressionDetail? BreathWeaponProgression { get; }

    public SavageAttacksDetail? SavageAttacks { get; }

    public RelentlessEnduranceDetail? RelentlessEndurance { get; }

    public LuckyDetail? Lucky { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
}
