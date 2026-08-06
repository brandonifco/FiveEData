using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Expenses.FoodAndLodging;
using FiveEData.Rules.Expenses.Lifestyles;

namespace FiveEData.Tests;

public sealed class
    LifestyleHospitalityCostCatalogIntegrityTests
{
    [Fact]
    public void ValidHospitalityReferences_HaveNoErrors()
    {
        var sourceId = new SourceDocumentId(
            "dnd5e2014.source.phb-first-printing");
        var ruleId = new RuleId(
            "dnd5e2014.expense-rule." +
            "food-drink-lodging-included-in-lifestyle");
        var lifestyleId = new LifestyleId(
            "dnd5e2014.lifestyle.modest");
        var lifestyleRuleId = new RuleId(
            "dnd5e2014.lifestyle-rule." +
            "modest-conditions");

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    CreateHospitality(
                        lifestyleId,
                        sourceId,
                        ruleId),
                    lifestyles:
                    [
                        CreateLifestyle(
                            lifestyleId,
                            sourceId,
                            lifestyleRuleId)
                    ],
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
                            "Test expense rule",
                            [
                                new SourceReference(
                                    sourceId,
                                    page: 158)
                            ]),
                        new RuleDefinition(
                            lifestyleRuleId,
                            "Modest lifestyle conditions",
                            [
                                new SourceReference(
                                    sourceId,
                                    page: 157)
                            ])
                    ]));

        Assert.Empty(errors);
    }

    [Fact]
    public void MissingLifestyleReference_IsRejected()
    {
        var sourceId = new SourceDocumentId(
            "dnd5e2014.source.phb-first-printing");

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    CreateHospitality(
                        new LifestyleId(
                            "dnd5e2014.lifestyle.missing"),
                        sourceId,
                        ruleId: null),
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
                    "Hospitality cost",
                    StringComparison.Ordinal) &&
                error.Contains(
                    "references missing lifestyle",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void MissingSourceReference_IsRejected()
    {
        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    CreateHospitality(
                        new LifestyleId(
                            "dnd5e2014.lifestyle.modest"),
                        new SourceDocumentId(
                            "dnd5e2014.source.missing"),
                        ruleId: null)));

        Assert.Contains(
            errors,
            error =>
                error.Contains(
                    "Hospitality cost",
                    StringComparison.Ordinal) &&
                error.Contains(
                    "missing source document",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void MissingRuleReference_IsRejected()
    {
        var sourceId = new SourceDocumentId(
            "dnd5e2014.source.phb-first-printing");
        var lifestyleId = new LifestyleId(
            "dnd5e2014.lifestyle.modest");

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    CreateHospitality(
                        lifestyleId,
                        sourceId,
                        new RuleId(
                            "dnd5e2014.expense-rule.missing")),
                    lifestyles:
                    [
                        CreateLifestyle(
                            lifestyleId,
                            sourceId)
                    ],
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
                    "Hospitality cost",
                    StringComparison.Ordinal) &&
                error.Contains(
                    "references missing rule",
                    StringComparison.Ordinal));
    }

    private static LifestyleHospitalityCostDefinition
        CreateHospitality(
            LifestyleId lifestyleId,
            SourceDocumentId sourceId,
            RuleId? ruleId)
    {
        return new LifestyleHospitalityCostDefinition(
            lifestyleId,
            new Money(50),
            new Money(30),
            ruleId is null ? [] : [ruleId.Value],
            [
                new SourceReference(
                    sourceId,
                    page: 158)
            ]);
    }

    private static LifestyleDefinition CreateLifestyle(
        LifestyleId lifestyleId,
        SourceDocumentId sourceId,
        RuleId? ruleId = null)
    {
        return new LifestyleDefinition(
            lifestyleId,
            "Test lifestyle",
            new ListedCost(
                new Money(100),
                ListedCostKind.Exact),
            specialRuleIds:
                ruleId is null ? [] : [ruleId.Value],
            sources:
            [
                new SourceReference(
                    sourceId,
                    page: 157)
            ]);
    }

    private static RulesetDefinitionSet CreateDefinitionSet(
        LifestyleHospitalityCostDefinition hospitality,
        IReadOnlyList<LifestyleDefinition>? lifestyles = null,
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
            lifestyles: lifestyles ?? [],
            foodAndDrink: [],
            hospitalityCosts: [hospitality],
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
            battleMasterManeuvers: [],
            eldritchInvocations: [],
            spellSlotProgressions: [],
            extraAttackProgressions: [],
            backgrounds: []);
    }
}
