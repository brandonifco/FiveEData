using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Expenses.Services;

namespace FiveEData.Tests;

public sealed class MundaneServiceCatalogIntegrityTests
{
    private static readonly SourceDocumentId SourceId =
        new("dnd5e2014.source.phb-first-printing");

    private static readonly RuleId ProficiencyRuleId =
        new(
            "dnd5e2014.expense-rule." +
            "skilled-hireling-proficiency-service");

    private static readonly RuleId MinimumPayRuleId =
        new(
            "dnd5e2014.expense-rule." +
            "skilled-hireling-pay-minimum");

    private static readonly RuleId MenialWorkRuleId =
        new(
            "dnd5e2014.expense-rule." +
            "untrained-hireling-menial-work");

    [Fact]
    public void ValidSkilledHirelingReferences_HaveNoErrors()
    {
        MundaneServiceDefinition service =
            CreateService(
                "dnd5e2014.mundane-service.hireling-skilled",
                [ProficiencyRuleId, MinimumPayRuleId]);

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    [service],
                    sources:
                    [
                        new SourceDocument(
                            SourceId,
                            "Player's Handbook")
                    ],
                    rules:
                    [
                        CreateRule(ProficiencyRuleId),
                        CreateRule(MinimumPayRuleId)
                    ]));

        Assert.Empty(errors);
    }

    [Fact]
    public void MissingSourceReference_IsRejected()
    {
        MundaneServiceDefinition service =
            CreateService(
                "dnd5e2014.mundane-service.messenger",
                [],
                new SourceDocumentId(
                    "dnd5e2014.source.missing"));

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet([service]));

        Assert.Contains(
            errors,
            error =>
                error.Contains(
                    "Mundane service",
                    StringComparison.Ordinal) &&
                error.Contains(
                    "missing source document",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void MissingRuleReference_IsRejected()
    {
        MundaneServiceDefinition service =
            CreateService(
                "dnd5e2014.mundane-service.hireling-untrained",
                [MenialWorkRuleId]);

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    [service],
                    sources:
                    [
                        new SourceDocument(
                            SourceId,
                            "Player's Handbook")
                    ]));

        Assert.Contains(
            errors,
            error => error.Contains(
                "references missing rule",
                StringComparison.Ordinal));
    }

    [Fact]
    public void SkilledHirelingMissingRequiredRule_IsRejected()
    {
        MundaneServiceDefinition service =
            CreateService(
                "dnd5e2014.mundane-service.hireling-skilled",
                [ProficiencyRuleId]);

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    [service],
                    sources:
                    [
                        new SourceDocument(
                            SourceId,
                            "Player's Handbook")
                    ],
                    rules:
                    [
                        CreateRule(ProficiencyRuleId),
                        CreateRule(MinimumPayRuleId)
                    ]));

        Assert.Contains(
            errors,
            error =>
                error.Contains(
                    "missing required rule association",
                    StringComparison.Ordinal) &&
                error.Contains(
                    MinimumPayRuleId.Value,
                    StringComparison.Ordinal));
    }

    [Fact]
    public void OrdinaryServiceUnexpectedRule_IsRejected()
    {
        var unexpectedRuleId =
            new RuleId(
                "dnd5e2014.expense-rule.unexpected");

        MundaneServiceDefinition service =
            CreateService(
                "dnd5e2014.mundane-service.messenger",
                [unexpectedRuleId]);

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    [service],
                    sources:
                    [
                        new SourceDocument(
                            SourceId,
                            "Player's Handbook")
                    ],
                    rules:
                    [
                        CreateRule(unexpectedRuleId)
                    ]));

        Assert.Contains(
            errors,
            error => error.Contains(
                "forbidden rule association",
                StringComparison.Ordinal));
    }

    [Fact]
    public void UntrainedHirelingRequiresMenialWorkRule()
    {
        MundaneServiceDefinition service =
            CreateService(
                "dnd5e2014.mundane-service.hireling-untrained",
                []);

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    [service],
                    sources:
                    [
                        new SourceDocument(
                            SourceId,
                            "Player's Handbook")
                    ],
                    rules:
                    [
                        CreateRule(MenialWorkRuleId)
                    ]));

        Assert.Contains(
            errors,
            error =>
                error.Contains(
                    "missing required rule association",
                    StringComparison.Ordinal) &&
                error.Contains(
                    MenialWorkRuleId.Value,
                    StringComparison.Ordinal));
    }

    [Fact]
    public void NoncanonicalIdentity_RemainsExtensionFriendly()
    {
        var extensionRuleId =
            new RuleId(
                "example.mundane-service-rule.custom");

        MundaneServiceDefinition service =
            CreateService(
                "example.mundane-service.custom",
                [extensionRuleId]);

        IReadOnlyList<string> errors =
            CatalogIntegrityValidator.Validate(
                CreateDefinitionSet(
                    [service],
                    sources:
                    [
                        new SourceDocument(
                            SourceId,
                            "Player's Handbook")
                    ],
                    rules:
                    [
                        CreateRule(extensionRuleId)
                    ]));

        Assert.Empty(errors);
    }

    private static MundaneServiceDefinition CreateService(
        string id,
        IEnumerable<RuleId> ruleIds,
        SourceDocumentId? sourceId = null)
    {
        return new MundaneServiceDefinition(
            new MundaneServiceId(id),
            "Test service",
            new ListedCost(
                new Money(100),
                ListedCostKind.Exact),
            ServicePricingUnit.Day,
            ruleIds,
            [
                new SourceReference(
                    sourceId ?? SourceId,
                    page: 159)
            ]);
    }

    private static RuleDefinition CreateRule(RuleId id)
    {
        return new RuleDefinition(
            id,
            "Test service rule",
            [
                new SourceReference(
                    SourceId,
                    page: 159)
            ]);
    }

    private static RulesetDefinitionSet CreateDefinitionSet(
        IReadOnlyList<MundaneServiceDefinition> services,
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
            foodAndDrink: [],
            hospitalityCosts: [],
            mundaneServices: services);

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
            spellSlotProgressions: [],
            extraAttackProgressions: [],
            backgrounds: [],
            magicSchools: [],
            spells: [],
            combatActions: []);
    }
}
