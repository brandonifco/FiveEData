using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Creatures.Abilities;
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

    private static SourceReference CreateSource()
    {
        return new SourceReference(
            SourceId,
            page: 174);
    }

    private static RulesetDefinitionSet CreateDefinitionSet(
        IReadOnlyList<AbilityDefinition> abilities,
        IReadOnlyList<SkillDefinition>? skills = null,
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
                skills: skills ?? []);

        return new RulesetDefinitionSet(
            sourceDocuments: sourceDocuments ?? [],
            rules: [],
            equipment: equipment,
            expenses: expenses,
            creatureVocabulary: creatureVocabulary);
    }
}
