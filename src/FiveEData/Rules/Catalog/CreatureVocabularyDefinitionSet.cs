using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Alignments;
using FiveEData.Rules.Creatures.Conditions;
using FiveEData.Rules.Creatures.DamageTypes;
using FiveEData.Rules.Creatures.Languages;
using FiveEData.Rules.Creatures.Senses;
using FiveEData.Rules.Creatures.Sizes;
using FiveEData.Rules.Creatures.Skills;

namespace FiveEData.Rules.Catalog;

internal sealed class CreatureVocabularyDefinitionSet
{
    public CreatureVocabularyDefinitionSet(
        IReadOnlyList<AbilityDefinition> abilities,
        IReadOnlyList<SkillDefinition> skills,
        IReadOnlyList<LanguageDefinition> languages,
        IReadOnlyList<CreatureSizeDefinition> sizes,
        IReadOnlyList<ConditionDefinition> conditions,
        IReadOnlyList<DamageTypeDefinition> damageTypes,
        IReadOnlyList<SenseDefinition> senses,
        IReadOnlyList<AlignmentDefinition> alignments)
    {
        ArgumentNullException.ThrowIfNull(abilities);
        ArgumentNullException.ThrowIfNull(skills);
        ArgumentNullException.ThrowIfNull(languages);
        ArgumentNullException.ThrowIfNull(sizes);
        ArgumentNullException.ThrowIfNull(conditions);
        ArgumentNullException.ThrowIfNull(damageTypes);
        ArgumentNullException.ThrowIfNull(senses);
        ArgumentNullException.ThrowIfNull(alignments);

        Abilities = abilities;
        Skills = skills;
        Languages = languages;
        Sizes = sizes;
        Conditions = conditions;
        DamageTypes = damageTypes;
        Senses = senses;
        Alignments = alignments;
    }

    public IReadOnlyList<AbilityDefinition> Abilities { get; }
    public IReadOnlyList<SkillDefinition> Skills { get; }
    public IReadOnlyList<LanguageDefinition> Languages { get; }
    public IReadOnlyList<CreatureSizeDefinition> Sizes { get; }
    public IReadOnlyList<ConditionDefinition> Conditions { get; }
    public IReadOnlyList<DamageTypeDefinition> DamageTypes { get; }
    public IReadOnlyList<SenseDefinition> Senses { get; }
    public IReadOnlyList<AlignmentDefinition> Alignments { get; }
}
