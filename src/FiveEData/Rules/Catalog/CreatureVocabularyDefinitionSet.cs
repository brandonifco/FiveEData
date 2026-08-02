using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Skills;

namespace FiveEData.Rules.Catalog;

internal sealed class CreatureVocabularyDefinitionSet
{
    public CreatureVocabularyDefinitionSet(
        IReadOnlyList<AbilityDefinition> abilities,
        IReadOnlyList<SkillDefinition> skills)
    {
        ArgumentNullException.ThrowIfNull(abilities);
        ArgumentNullException.ThrowIfNull(skills);

        Abilities = abilities;
        Skills = skills;
    }

    public IReadOnlyList<AbilityDefinition> Abilities { get; }
    public IReadOnlyList<SkillDefinition> Skills { get; }
}
