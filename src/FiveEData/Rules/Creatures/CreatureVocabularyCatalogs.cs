using FiveEData.Rules.Catalog;

namespace FiveEData.Rules.Creatures;

public sealed class CreatureVocabularyCatalogs
{
    internal CreatureVocabularyCatalogs(
        AbilityCatalog abilities,
        SkillCatalog skills,
        LanguageCatalog languages,
        CreatureSizeCatalog sizes)
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

    public AbilityCatalog Abilities { get; }
    public SkillCatalog Skills { get; }
    public LanguageCatalog Languages { get; }
    public CreatureSizeCatalog Sizes { get; }
}
