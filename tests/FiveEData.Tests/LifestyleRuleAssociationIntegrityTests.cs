using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Expenses.Lifestyles;

namespace FiveEData.Tests;

public sealed class
    LifestyleRuleAssociationIntegrityTests
{
    private static readonly SourceDocumentId SourceId =
        new("dnd5e2014.source.phb-first-printing");

    private static readonly RuleId ModestRuleId =
        new(
            "dnd5e2014.lifestyle-rule." +
            "modest-conditions");

    private static readonly RuleId PoorRuleId =
        new(
            "dnd5e2014.lifestyle-rule." +
            "poor-conditions");

    private static readonly RuleId UnexpectedRuleId =
        new("dnd5e2014.lifestyle-rule.unexpected");

    [Fact]
    public void CanonicalAssociationMatrix_HasNoErrors()
    {
        (string Id, string RuleId)[] expected =
        [
            (
                "dnd5e2014.lifestyle.wretched",
                "dnd5e2014.lifestyle-rule." +
                "wretched-conditions"
            ),
            (
                "dnd5e2014.lifestyle.squalid",
                "dnd5e2014.lifestyle-rule." +
                "squalid-conditions"
            ),
            (
                "dnd5e2014.lifestyle.poor",
                "dnd5e2014.lifestyle-rule." +
                "poor-conditions"
            ),
            (
                "dnd5e2014.lifestyle.modest",
                "dnd5e2014.lifestyle-rule." +
                "modest-conditions"
            ),
            (
                "dnd5e2014.lifestyle.comfortable",
                "dnd5e2014.lifestyle-rule." +
                "comfortable-conditions"
            ),
            (
                "dnd5e2014.lifestyle.wealthy",
                "dnd5e2014.lifestyle-rule." +
                "wealthy-conditions"
            ),
            (
                "dnd5e2014.lifestyle.aristocratic",
                "dnd5e2014.lifestyle-rule." +
                "aristocratic-conditions"
            )
        ];

        IReadOnlyList<LifestyleDefinition> lifestyles =
            expected
                .Select(
                    item =>
                        CreateLifestyle(
                            item.Id,
                            [new RuleId(item.RuleId)]))
                .ToArray();

        IReadOnlyList<RuleDefinition> rules =
            expected
                .Select(
                    item =>
                        CreateRule(
                            new RuleId(item.RuleId)))
                .ToArray();

        Assert.Empty(
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    lifestyles,
                    rules)));
    }

    [Fact]
    public void MissingCanonicalAssociation_IsRejected()
    {
        LifestyleDefinition lifestyle =
            CreateLifestyle(
                "dnd5e2014.lifestyle.modest",
                ruleIds: []);

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    [lifestyle],
                    [CreateRule(ModestRuleId)]));

        Assert.Contains(
            errors,
            error =>
                error.Contains(
                    "missing required rule association",
                    StringComparison.Ordinal) &&
                error.Contains(
                    ModestRuleId.Value,
                    StringComparison.Ordinal));
    }

    [Fact]
    public void WrongCanonicalAssociation_IsRejected()
    {
        LifestyleDefinition lifestyle =
            CreateLifestyle(
                "dnd5e2014.lifestyle.modest",
                [PoorRuleId]);

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    [lifestyle],
                    [CreateRule(PoorRuleId)]));

        Assert.Contains(
            errors,
            error =>
                error.Contains(
                    "missing required rule association",
                    StringComparison.Ordinal) &&
                error.Contains(
                    ModestRuleId.Value,
                    StringComparison.Ordinal));

        Assert.Contains(
            errors,
            error =>
                error.Contains(
                    "forbidden rule association",
                    StringComparison.Ordinal) &&
                error.Contains(
                    PoorRuleId.Value,
                    StringComparison.Ordinal));
    }

    [Fact]
    public void AdditionalCanonicalAssociation_IsRejected()
    {
        LifestyleDefinition lifestyle =
            CreateLifestyle(
                "dnd5e2014.lifestyle.modest",
                [ModestRuleId, UnexpectedRuleId]);

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    [lifestyle],
                    [
                        CreateRule(ModestRuleId),
                        CreateRule(UnexpectedRuleId)
                    ]));

        Assert.Contains(
            errors,
            error =>
                error.Contains(
                    "forbidden rule association",
                    StringComparison.Ordinal) &&
                error.Contains(
                    UnexpectedRuleId.Value,
                    StringComparison.Ordinal));
    }

    [Fact]
    public void NoncanonicalIdentity_RemainsExtensionFriendly()
    {
        var extensionRuleId =
            new RuleId(
                "example.lifestyle-rule.custom");

        LifestyleDefinition lifestyle =
            CreateLifestyle(
                "example.lifestyle.custom",
                [extensionRuleId]);

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    [lifestyle],
                    [CreateRule(extensionRuleId)]));

        Assert.Empty(errors);
    }

    private static LifestyleDefinition CreateLifestyle(
        string id,
        IEnumerable<RuleId> ruleIds)
    {
        return new LifestyleDefinition(
            new LifestyleId(id),
            "Test lifestyle",
            new ListedCost(
                new Money(100),
                ListedCostKind.Exact),
            ruleIds,
            [
                new SourceReference(
                    SourceId,
                    page: 157)
            ]);
    }

    private static RuleDefinition CreateRule(RuleId id)
    {
        return new RuleDefinition(
            id,
            "Test lifestyle rule",
            [
                new SourceReference(
                    SourceId,
                    page: 157)
            ]);
    }

    private static RulesetDefinitionSet CreateDefinitionSet(
        IReadOnlyList<LifestyleDefinition> lifestyles,
        IReadOnlyList<RuleDefinition> rules)
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
            lifestyles: lifestyles,
            foodAndDrink: [],
            hospitalityCosts: [],
            mundaneServices: []);

        return new RulesetDefinitionSet(
            sourceDocuments:
            [
                new SourceDocument(
                    SourceId,
                    "Player's Handbook")
            ],
            rules: rules,
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
