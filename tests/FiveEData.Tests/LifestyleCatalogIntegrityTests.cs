using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Expenses.Lifestyles;

namespace FiveEData.Tests;

public sealed class LifestyleCatalogIntegrityTests
{
    [Fact]
    public void ValidLifestyleReferences_HaveNoErrors()
    {
        var sourceId = new SourceDocumentId(
            "dnd5e2014.source.phb-first-printing");
        var ruleId = new RuleId(
            "dnd5e2014.lifestyle-rule.test");

        LifestyleDefinition lifestyle =
            CreateLifestyle(sourceId, ruleId);

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    lifestyle,
                    sources:
                    [
                        new SourceDocument(
                            sourceId,
                            "Player's Handbook")
                    ],
                    rules:
                    [
                        new RuleDefinition(
                            ruleId,
                            "Test lifestyle rule",
                            [
                                new SourceReference(
                                    sourceId,
                                    page: 157)
                            ])
                    ]));

        Assert.Empty(errors);
    }

    [Fact]
    public void MissingLifestyleSourceReference_IsRejected()
    {
        LifestyleDefinition lifestyle =
            CreateLifestyle(
                new SourceDocumentId(
                    "dnd5e2014.source.missing"),
                ruleId: null);

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(lifestyle));

        Assert.Contains(
            errors,
            error =>
                error.Contains(
                    "Lifestyle",
                    StringComparison.Ordinal) &&
                error.Contains(
                    "missing source document",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void MissingLifestyleRuleReference_IsRejected()
    {
        var sourceId = new SourceDocumentId(
            "dnd5e2014.source.phb-first-printing");

        LifestyleDefinition lifestyle =
            CreateLifestyle(
                sourceId,
                new RuleId(
                    "dnd5e2014.lifestyle-rule.missing"));

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    lifestyle,
                    sources:
                    [
                        new SourceDocument(
                            sourceId,
                            "Player's Handbook")
                    ]));

        Assert.Contains(
            errors,
            error =>
                error.Contains(
                    "Lifestyle",
                    StringComparison.Ordinal) &&
                error.Contains(
                    "references missing rule",
                    StringComparison.Ordinal));
    }

    private static LifestyleDefinition CreateLifestyle(
        SourceDocumentId sourceId,
        RuleId? ruleId)
    {
        return new LifestyleDefinition(
            new LifestyleId(
                "dnd5e2014.lifestyle.test"),
            "Test lifestyle",
            new ListedCost(
                new Money(100),
                ListedCostKind.Exact),
            ruleId is null ? [] : [ruleId.Value],
            [
                new SourceReference(
                    sourceId,
                    page: 157)
            ]);
    }

    private static RulesetDefinitionSet CreateDefinitionSet(
        LifestyleDefinition lifestyle,
        IReadOnlyList<SourceDocument>? sources = null,
        IReadOnlyList<RuleDefinition>? rules = null)
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
            lifestyles: [lifestyle],
            foodAndDrink: [],
            hospitalityCosts: [],
            mundaneServices: []);

        return new RulesetDefinitionSet(
            sourceDocuments: sources ?? [],
            rules: rules ?? [],
            equipment: equipment,
            expenses: expenses,
            creatureVocabulary:
                new CreatureVocabularyDefinitionSet(
                    abilities: [],
                    skills: [],
                    languages: [],
                    sizes: [],
                    conditions: [],
                    damageTypes: [],
                    senses: [],
                    alignments: []),
            races: new RaceDefinitionSet(races: [], subraces: []),
            classes: new ClassDefinitionSet(classes: [], subclasses: []),
            fightingStyles: [],
            metamagicOptions: [],
            spellSlotProgressions: [],
            extraAttackProgressions: [],
            backgrounds: []);
    }
}
