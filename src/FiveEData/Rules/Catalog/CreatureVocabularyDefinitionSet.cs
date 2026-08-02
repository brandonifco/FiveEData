using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Languages;
using FiveEData.Rules.Creatures.Sizes;
using FiveEData.Rules.Creatures.Skills;

namespace FiveEData.Rules.Catalog;

internal sealed class CreatureVocabularyDefinitionSet
{
    public CreatureVocabularyDefinitionSet(
        IReadOnlyList<AbilityDefinition> abilities,
        IReadOnlyList<SkillDefinition> skills,
        IReadOnlyList<LanguageDefinition> languages,
        IReadOnlyList<CreatureSizeDefinition> sizes)
    {
        ArgumentNullException.ThrowIfNull(abilities);
        ArgumentNullException.ThrowIfNull(skills);
        ArgumentNullException.ThrowIfNull(languages);
        ArgumentNullException.ThrowIfNull(sizes);

        Abilities = abilities;
        Skills = skills;
        Languages = languages;
        Sizes = sizes;
    }

    public IReadOnlyList<AbilityDefinition> Abilities { get; }
    public IReadOnlyList<SkillDefinition> Skills { get; }
    public IReadOnlyList<LanguageDefinition> Languages { get; }
    public IReadOnlyList<CreatureSizeDefinition> Sizes { get; }
}
