using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Expenses.FoodAndLodging;

namespace FiveEData.Tests;

public sealed class FoodDrinkCatalogIntegrityTests
{
    [Fact]
    public void ValidFoodDrinkReferences_HaveNoErrors()
    {
        var sourceId = new SourceDocumentId(
            "dnd5e2014.source.phb-first-printing");
        var ruleId = new RuleId(
            "dnd5e2014.expense-rule." +
            "food-drink-lodging-included-in-lifestyle");

        FoodDrinkDefinition definition =
            CreateDefinition(sourceId, ruleId);

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    definition,
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
                            ])
                    ]));

        Assert.Empty(errors);
    }

    [Fact]
    public void MissingFoodDrinkSourceReference_IsRejected()
    {
        FoodDrinkDefinition definition =
            CreateDefinition(
                new SourceDocumentId(
                    "dnd5e2014.source.missing"),
                ruleId: null);

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(definition));

        Assert.Contains(
            errors,
            error =>
                error.Contains(
                    "Food and drink",
                    StringComparison.Ordinal) &&
                error.Contains(
                    "missing source document",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void MissingFoodDrinkRuleReference_IsRejected()
    {
        var sourceId = new SourceDocumentId(
            "dnd5e2014.source.phb-first-printing");

        FoodDrinkDefinition definition =
            CreateDefinition(
                sourceId,
                new RuleId(
                    "dnd5e2014.expense-rule.missing"));

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    definition,
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
                    "Food and drink",
                    StringComparison.Ordinal) &&
                error.Contains(
                    "references missing rule",
                    StringComparison.Ordinal));
    }

    private static FoodDrinkDefinition CreateDefinition(
        SourceDocumentId sourceId,
        RuleId? ruleId)
    {
        return new FoodDrinkDefinition(
            new FoodDrinkId(
                "dnd5e2014.food-drink.test"),
            "Test food",
            new Money(10),
            FoodDrinkPricingUnit.Loaf,
            ruleId is null ? [] : [ruleId.Value],
            [
                new SourceReference(
                    sourceId,
                    page: 158)
            ]);
    }

    private static RulesetDefinitionSet CreateDefinitionSet(
        FoodDrinkDefinition definition,
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
            lifestyles: [],
            foodAndDrink: [definition],
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
            battleMasterManeuvers: [],
            eldritchInvocations: [],
            elementalDisciplines: [],
            channelDivinityOptions: [],
            totemWarriorOptions: [],
            hunterOptions: [],
            openHandTechniqueOptions: [],
            thirdEyeOptions: [],
            transmutersStoneOptions: [],
            spellSlotProgressions: [],
            extraAttackProgressions: [],
            backgrounds: [],
            magicSchools: [],
            spells: [],
            combatActions: [],
            cover: [],
            travelPaces: [],
            restTypes: [],
            downtimeActivities: []);
    }
}
