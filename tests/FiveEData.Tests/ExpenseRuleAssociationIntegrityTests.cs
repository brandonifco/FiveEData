using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Expenses.FoodAndLodging;
using FiveEData.Rules.Expenses.Lifestyles;

namespace FiveEData.Tests;

public sealed class ExpenseRuleAssociationIntegrityTests
{
    private static readonly SourceDocumentId SourceId =
        new("dnd5e2014.source.phb-first-printing");

    private static readonly RuleId InclusionRuleId =
        new(
            "dnd5e2014.expense-rule." +
            "food-drink-lodging-included-in-lifestyle");

    private static readonly RuleId UnexpectedRuleId =
        new("dnd5e2014.expense-rule.unexpected");

    private static readonly LifestyleId ModestLifestyleId =
        new("dnd5e2014.lifestyle.modest");

    [Fact]
    public void FoodDrinkMissingRequiredAssociation_IsRejected()
    {
        RulesetDefinitionSet definitions =
            CreateDefinitionSet(
                foodAndDrink:
                [
                    CreateFoodDrink(ruleIds: [])
                ],
                rules:
                [
                    CreateRule(InclusionRuleId)
                ]);

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(definitions);

        Assert.Contains(
            "Food and drink 'dnd5e2014.food-drink.test' " +
            "is missing required rule association " +
            "'dnd5e2014.expense-rule." +
            "food-drink-lodging-included-in-lifestyle'.",
            errors);
    }

    [Fact]
    public void FoodDrinkUnexpectedAssociation_IsRejected()
    {
        RulesetDefinitionSet definitions =
            CreateDefinitionSet(
                foodAndDrink:
                [
                    CreateFoodDrink(
                        [InclusionRuleId, UnexpectedRuleId])
                ],
                rules:
                [
                    CreateRule(InclusionRuleId),
                    CreateRule(UnexpectedRuleId)
                ]);

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(definitions);

        Assert.Contains(
            "Food and drink 'dnd5e2014.food-drink.test' " +
            "has forbidden rule association " +
            "'dnd5e2014.expense-rule.unexpected'.",
            errors);
    }

    [Fact]
    public void HospitalityMissingRequiredAssociation_IsRejected()
    {
        RulesetDefinitionSet definitions =
            CreateDefinitionSet(
                hospitalityCosts:
                [
                    CreateHospitality(ruleIds: [])
                ],
                rules:
                [
                    CreateRule(InclusionRuleId)
                ]);

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(definitions);

        Assert.Contains(
            "Hospitality cost for lifestyle " +
            "'dnd5e2014.lifestyle.modest' " +
            "is missing required rule association " +
            "'dnd5e2014.expense-rule." +
            "food-drink-lodging-included-in-lifestyle'.",
            errors);
    }

    [Fact]
    public void HospitalityUnexpectedAssociation_IsRejected()
    {
        RulesetDefinitionSet definitions =
            CreateDefinitionSet(
                hospitalityCosts:
                [
                    CreateHospitality(
                        [InclusionRuleId, UnexpectedRuleId])
                ],
                rules:
                [
                    CreateRule(InclusionRuleId),
                    CreateRule(UnexpectedRuleId)
                ]);

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(definitions);

        Assert.Contains(
            "Hospitality cost for lifestyle " +
            "'dnd5e2014.lifestyle.modest' " +
            "has forbidden rule association " +
            "'dnd5e2014.expense-rule.unexpected'.",
            errors);
    }

    private static FoodDrinkDefinition CreateFoodDrink(
        IEnumerable<RuleId> ruleIds)
    {
        return new FoodDrinkDefinition(
            new FoodDrinkId(
                "dnd5e2014.food-drink.test"),
            "Test food",
            new Money(10),
            FoodDrinkPricingUnit.Loaf,
            ruleIds,
            [
                new SourceReference(
                    SourceId,
                    page: 158)
            ]);
    }

    private static LifestyleHospitalityCostDefinition
        CreateHospitality(IEnumerable<RuleId> ruleIds)
    {
        return new LifestyleHospitalityCostDefinition(
            ModestLifestyleId,
            new Money(50),
            new Money(30),
            ruleIds,
            [
                new SourceReference(
                    SourceId,
                    page: 158)
            ]);
    }

    private static LifestyleDefinition CreateLifestyle()
    {
        return new LifestyleDefinition(
            ModestLifestyleId,
            "Modest",
            new ListedCost(
                new Money(100),
                ListedCostKind.Exact),
            specialRuleIds: [],
            sources:
            [
                new SourceReference(
                    SourceId,
                    page: 157)
            ]);
    }

    private static RuleDefinition CreateRule(RuleId ruleId)
    {
        return new RuleDefinition(
            ruleId,
            "Test expense rule",
            [
                new SourceReference(
                    SourceId,
                    page: 158)
            ]);
    }

    private static RulesetDefinitionSet CreateDefinitionSet(
        IReadOnlyList<FoodDrinkDefinition>? foodAndDrink = null,
        IReadOnlyList<
            LifestyleHospitalityCostDefinition>?
                hospitalityCosts = null,
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
            lifestyles: [CreateLifestyle()],
            foodAndDrink: foodAndDrink ?? [],
            hospitalityCosts: hospitalityCosts ?? [],
            mundaneServices: []);

        return new RulesetDefinitionSet(
            sourceDocuments:
            [
                new SourceDocument(
                    SourceId,
                    "Player's Handbook")
            ],
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
            spellSlotProgressions: [],
            extraAttackProgressions: [],
            backgrounds: []);
    }
}
