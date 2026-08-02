using FiveEData.Rules.Catalog;

namespace FiveEData.Rules.Creatures;

public sealed class CreatureVocabularyCatalogs
{
    internal CreatureVocabularyCatalogs(
        AbilityCatalog abilities,
        SkillCatalog skills)
    {
        ArgumentNullException.ThrowIfNull(abilities);
        ArgumentNullException.ThrowIfNull(skills);

        Abilities = abilities;
        Skills = skills;
    }

    public AbilityCatalog Abilities { get; }
    public SkillCatalog Skills { get; }
}
