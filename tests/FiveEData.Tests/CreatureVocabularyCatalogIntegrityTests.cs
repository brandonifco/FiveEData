using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Alignments;
using FiveEData.Rules.Creatures.Conditions;
using FiveEData.Rules.Creatures.DamageTypes;
using FiveEData.Rules.Creatures.Languages;
using FiveEData.Rules.Creatures.Senses;
using FiveEData.Rules.Creatures.Sizes;
using FiveEData.Rules.Creatures.Skills;

namespace FiveEData.Tests;

public sealed class CreatureVocabularyCatalogIntegrityTests
{
    private static readonly SourceDocumentId SourceId =
        new("dnd5e2014.source.phb-first-printing");

    [Fact]
    public void ValidVocabularyReferences_HaveNoErrors()
    {
        AbilityDefinition ability = CreateAbility(
            "example.ability.agility",
            "Agility");

        SkillDefinition skill = CreateSkill(
            "example.skill.tumbling",
            "Tumbling",
            ability.Id);

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    abilities: [ability],
                    skills: [skill],
                    sourceDocuments:
                    [
                        new SourceDocument(
                            SourceId,
                            "Player's Handbook")
                    ]));

        Assert.Empty(errors);
    }

    [Fact]
    public void MissingAbilitySourceReference_IsRejected()
    {
        AbilityDefinition ability = CreateAbility(
            "example.ability.agility",
            "Agility");

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    abilities: [ability]));

        Assert.Contains(
            errors,
            error =>
                error.Contains(
                    "Ability 'example.ability.agility'",
                    StringComparison.Ordinal) &&
                error.Contains(
                    "missing source document",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void MissingSkillSourceReference_IsRejected()
    {
        AbilityDefinition ability = CreateAbility(
            "example.ability.agility",
            "Agility");

        SkillDefinition skill = CreateSkill(
            "example.skill.tumbling",
            "Tumbling",
            ability.Id);

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    abilities: [ability],
                    skills: [skill]));

        Assert.Contains(
            errors,
            error =>
                error.Contains(
                    "Skill 'example.skill.tumbling'",
                    StringComparison.Ordinal) &&
                error.Contains(
                    "missing source document",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void MissingLanguageSourceReference_IsRejected()
    {
        LanguageDefinition language = CreateLanguage(
            "example.language.trade-speech",
            "Trade Speech",
            LanguageCategory.Standard);

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    abilities: [],
                    languages: [language]));

        Assert.Contains(
            errors,
            error =>
                error.Contains(
                    "Language 'example.language.trade-speech'",
                    StringComparison.Ordinal) &&
                error.Contains(
                    "missing source document",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void MissingCreatureSizeSourceReference_IsRejected()
    {
        CreatureSizeDefinition size = CreateSize(
            "example.creature-size.colossal",
            "Colossal");

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    abilities: [],
                    sizes: [size]));

        Assert.Contains(
            errors,
            error =>
                error.Contains(
                    "Creature size " +
                    "'example.creature-size.colossal'",
                    StringComparison.Ordinal) &&
                error.Contains(
                    "missing source document",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void MissingConditionSourceReference_IsRejected()
    {
        ConditionDefinition condition = CreateCondition(
            "example.condition.dazed",
            "Dazed");

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    abilities: [],
                    conditions: [condition]));

        Assert.Contains(
            errors,
            error =>
                error.Contains(
                    "Condition 'example.condition.dazed'",
                    StringComparison.Ordinal) &&
                error.Contains(
                    "missing source document",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void MissingDamageTypeSourceReference_IsRejected()
    {
        DamageTypeDefinition damageType = CreateDamageType(
            "example.damage-type.sonic",
            "Sonic");

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    abilities: [],
                    damageTypes: [damageType]));

        Assert.Contains(
            errors,
            error =>
                error.Contains(
                    "Damage type 'example.damage-type.sonic'",
                    StringComparison.Ordinal) &&
                error.Contains(
                    "missing source document",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void MissingSenseSourceReference_IsRejected()
    {
        SenseDefinition sense = CreateSense(
            "example.sense.tremorsense",
            "Tremorsense");

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    abilities: [],
                    senses: [sense]));

        Assert.Contains(
            errors,
            error =>
                error.Contains(
                    "Sense 'example.sense.tremorsense'",
                    StringComparison.Ordinal) &&
                error.Contains(
                    "missing source document",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void MissingAlignmentSourceReference_IsRejected()
    {
        AlignmentDefinition alignment = CreateAlignment(
            "example.alignment.true-neutral",
            "True Neutral",
            AlignmentEthic.Neutral,
            AlignmentMorality.Neutral);

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    abilities: [],
                    alignments: [alignment]));

        Assert.Contains(
            errors,
            error =>
                error.Contains(
                    "Alignment 'example.alignment.true-neutral'",
                    StringComparison.Ordinal) &&
                error.Contains(
                    "missing source document",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void MissingNormallyAssociatedAbility_IsRejected()
    {
        SkillDefinition skill = CreateSkill(
            "example.skill.tumbling",
            "Tumbling",
            new AbilityId(
                "example.ability.missing"));

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    abilities: [],
                    skills: [skill],
                    sourceDocuments:
                    [
                        new SourceDocument(
                            SourceId,
                            "Player's Handbook")
                    ]));

        Assert.Contains(
            errors,
            error =>
                error.Contains(
                    "example.skill.tumbling",
                    StringComparison.Ordinal) &&
                error.Contains(
                    "missing normally associated ability",
                    StringComparison.Ordinal) &&
                error.Contains(
                    "example.ability.missing",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void NoncanonicalVocabulary_RemainsExtensionFriendly()
    {
        AbilityDefinition ability = CreateAbility(
            "example.ability.luck",
            "Luck");

        SkillDefinition skill = CreateSkill(
            "example.skill.fortune-telling",
            "Fortune Telling",
            ability.Id);

        LanguageDefinition language = CreateLanguage(
            "example.language.starsong",
            "Starsong",
            LanguageCategory.Exotic);

        CreatureSizeDefinition size = CreateSize(
            "example.creature-size.minuscule",
            "Minuscule");

        ConditionDefinition condition = CreateCondition(
            "example.condition.dazed",
            "Dazed");

        DamageTypeDefinition damageType = CreateDamageType(
            "example.damage-type.sonic",
            "Sonic");

        SenseDefinition sense = CreateSense(
            "example.sense.tremorsense",
            "Tremorsense");

        AlignmentDefinition alignment = CreateAlignment(
            "example.alignment.true-neutral",
            "True Neutral",
            AlignmentEthic.Neutral,
            AlignmentMorality.Neutral);

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    abilities: [ability],
                    skills: [skill],
                    languages: [language],
                    sizes: [size],
                    conditions: [condition],
                    damageTypes: [damageType],
                    senses: [sense],
                    alignments: [alignment],
                    sourceDocuments:
                    [
                        new SourceDocument(
                            SourceId,
                            "Player's Handbook")
                    ]));

        Assert.Empty(errors);
    }

    private static AbilityDefinition CreateAbility(
        string id,
        string name)
    {
        return new AbilityDefinition(
            new AbilityId(id),
            name,
            [CreateSource()]);
    }

    private static SkillDefinition CreateSkill(
        string id,
        string name,
        AbilityId abilityId)
    {
        return new SkillDefinition(
            new SkillId(id),
            name,
            abilityId,
            [CreateSource()]);
    }

    private static LanguageDefinition CreateLanguage(
        string id,
        string name,
        LanguageCategory category)
    {
        return new LanguageDefinition(
            new LanguageId(id),
            name,
            category,
            [CreateSource()]);
    }

    private static CreatureSizeDefinition CreateSize(
        string id,
        string name)
    {
        return new CreatureSizeDefinition(
            new CreatureSizeId(id),
            name,
            [CreateSource()]);
    }

    private static ConditionDefinition CreateCondition(
        string id,
        string name)
    {
        return new ConditionDefinition(
            new ConditionId(id),
            name,
            [CreateSource()]);
    }

    private static DamageTypeDefinition CreateDamageType(
        string id,
        string name)
    {
        return new DamageTypeDefinition(
            new DamageTypeId(id),
            name,
            [CreateSource()]);
    }

    private static SenseDefinition CreateSense(
        string id,
        string name)
    {
        return new SenseDefinition(
            new SenseId(id),
            name,
            [CreateSource()]);
    }

    private static AlignmentDefinition CreateAlignment(
        string id,
        string name,
        AlignmentEthic ethic,
        AlignmentMorality morality)
    {
        return new AlignmentDefinition(
            new AlignmentId(id),
            name,
            ethic,
            morality,
            [CreateSource()]);
    }

    private static SourceReference CreateSource()
    {
        return new SourceReference(
            SourceId,
            page: 174);
    }

    private static RulesetDefinitionSet CreateDefinitionSet(
        IReadOnlyList<AbilityDefinition> abilities,
        IReadOnlyList<SkillDefinition>? skills = null,
        IReadOnlyList<LanguageDefinition>? languages = null,
        IReadOnlyList<CreatureSizeDefinition>? sizes = null,
        IReadOnlyList<ConditionDefinition>? conditions = null,
        IReadOnlyList<DamageTypeDefinition>? damageTypes = null,
        IReadOnlyList<SenseDefinition>? senses = null,
        IReadOnlyList<AlignmentDefinition>? alignments = null,
        IReadOnlyList<SourceDocument>? sourceDocuments = null)
    {
        var equipment = new EquipmentDefinitionSet(
            weapons: [],
            ammunition: [],
            armor: [],
            shields: [],
            adventuringGear: [],
            containerCapacities: [],
            toolFamilies: [],
            tools: [],
            mounts: [],
            vehicles: [],
            mountSupport: [],
            tradeGoods: []);

        var expenses = new ExpenseDefinitionSet(
            lifestyles: [],
            foodAndDrink: [],
            hospitalityCosts: [],
            mundaneServices: []);

        var creatureVocabulary =
            new CreatureVocabularyDefinitionSet(
                abilities: abilities,
                skills: skills ?? [],
                languages: languages ?? [],
                sizes: sizes ?? [],
                conditions: conditions ?? [],
                damageTypes: damageTypes ?? [],
                senses: senses ?? [],
                alignments: alignments ?? []);

        return new RulesetDefinitionSet(
            sourceDocuments: sourceDocuments ?? [],
            rules: [],
            equipment: equipment,
            expenses: expenses,
            creatureVocabulary: creatureVocabulary,
            races: new RaceDefinitionSet(races: [], subraces: []),
            classes: new ClassDefinitionSet(classes: [], subclasses: []),
            fightingStyles: [],
            metamagicOptions: [],
            spellSlotProgressions: [],
            extraAttackProgressions: [],
            backgrounds: []);
    }
}
