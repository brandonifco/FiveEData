using FiveEData.Rules.Catalog;

namespace FiveEData.Rules.Creatures;

public sealed class CreatureVocabularyCatalogs
{
    internal CreatureVocabularyCatalogs(
        AbilityCatalog abilities,
        SkillCatalog skills,
        LanguageCatalog languages,
        CreatureSizeCatalog sizes,
        ConditionCatalog conditions,
        DamageTypeCatalog damageTypes,
        SenseCatalog senses,
        AlignmentCatalog alignments)
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

    public AbilityCatalog Abilities { get; }
    public SkillCatalog Skills { get; }
    public LanguageCatalog Languages { get; }
    public CreatureSizeCatalog Sizes { get; }
    public ConditionCatalog Conditions { get; }
    public DamageTypeCatalog DamageTypes { get; }
    public SenseCatalog Senses { get; }
    public AlignmentCatalog Alignments { get; }
}
