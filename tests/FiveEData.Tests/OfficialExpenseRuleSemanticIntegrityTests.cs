using FiveEData.Rules.Catalog;
using FiveEData.Rules.Creatures.Abilities.Serialization;
using FiveEData.Rules.Creatures.Alignments.Serialization;
using FiveEData.Rules.Creatures.Conditions.Serialization;
using FiveEData.Rules.Creatures.DamageTypes.Serialization;
using FiveEData.Rules.Creatures.Languages.Serialization;
using FiveEData.Rules.Creatures.Senses.Serialization;
using FiveEData.Rules.Creatures.Sizes.Serialization;
using FiveEData.Rules.Creatures.Skills.Serialization;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Equipment.Armor.Serialization;
using FiveEData.Rules.Equipment.Mounts.Serialization;
using FiveEData.Rules.Equipment.MountSupport.Serialization;
using FiveEData.Rules.Equipment.MountsAndVehicles.Serialization;
using FiveEData.Rules.Equipment.Vehicles.Serialization;
using FiveEData.Rules.Expenses;
using FiveEData.Rules.Expenses.FoodAndLodging.Serialization;
using FiveEData.Rules.Expenses.Lifestyles.Serialization;
using FiveEData.Rules.Expenses.Services.Serialization;

namespace FiveEData.Tests;

public sealed class OfficialExpenseRuleSemanticIntegrityTests
{
    private static readonly RuleId MissingSpellcastingRuleId =
        new(
            "dnd5e2014.expense-rule." +
            "spellcasting-services-no-established-rates");

    [Fact]
    public void CanonicalRules_HaveNoErrors()
    {
        Assert.Empty(
            OfficialExpenseRuleSemanticValidator.Validate(
                LoadCanonicalRules()));
    }

    [Fact]
    public void MissingUnreferencedSpellcastingRule_IsRejected()
    {
        IReadOnlyList<RuleDefinition> altered =
            LoadCanonicalRules()
                .Where(
                    rule =>
                        rule.Id != MissingSpellcastingRuleId)
                .ToArray();

        IReadOnlyList<string> errors =
            OfficialExpenseRuleSemanticValidator.Validate(
                altered);

        Assert.Contains(
            errors,
            error =>
                error.Contains(
                    "exactly 23 managed definitions; found 22",
                    StringComparison.Ordinal));

        Assert.Contains(
            errors,
            error =>
                error.Contains(
                    "is missing",
                    StringComparison.Ordinal) &&
                error.Contains(
                    MissingSpellcastingRuleId.Value,
                    StringComparison.Ordinal));
    }

    [Fact]
    public void UnexpectedManagedRule_IsRejected()
    {
        IReadOnlyList<RuleDefinition> canonical =
            LoadCanonicalRules();

        var unexpectedId =
            new RuleId(
                "dnd5e2014.expense-rule.unexpected");

        var unexpected =
            new RuleDefinition(
                unexpectedId,
                "Unexpected expense rule",
                canonical[0].Sources);

        IReadOnlyList<string> errors =
            OfficialExpenseRuleSemanticValidator.Validate(
                canonical
                    .Append(unexpected)
                    .ToArray());

        Assert.Contains(
            errors,
            error =>
                error.Contains(
                    "unexpected definition",
                    StringComparison.Ordinal) &&
                error.Contains(
                    unexpectedId.Value,
                    StringComparison.Ordinal));
    }

    [Fact]
    public void AlteredRuleName_IsRejected()
    {
        IReadOnlyList<RuleDefinition> canonical =
            LoadCanonicalRules();

        var targetId =
            new RuleId(
                "dnd5e2014.expense-rule.self-sufficiency");

        RuleDefinition target =
            canonical.Single(rule => rule.Id == targetId);

        var alteredRule =
            new RuleDefinition(
                target.Id,
                "Altered self-sufficiency rule",
                target.Sources);

        IReadOnlyList<string> errors =
            OfficialExpenseRuleSemanticValidator.Validate(
                ReplaceRule(
                    canonical,
                    alteredRule));

        Assert.Contains(
            errors,
            error =>
                error.Contains(
                    targetId.Value,
                    StringComparison.Ordinal) &&
                error.Contains(
                    "must be named",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void AlteredRuleProvenance_IsRejected()
    {
        IReadOnlyList<RuleDefinition> canonical =
            LoadCanonicalRules();

        RuleDefinition target =
            canonical.Single(
                rule =>
                    rule.Id == MissingSpellcastingRuleId);

        var alteredRule =
            new RuleDefinition(
                target.Id,
                target.Name,
                [
                    new SourceReference(
                        target.Sources[0].DocumentId,
                        page: 158,
                        section:
                            "Chapter 5: Equipment — Expenses — " +
                            "Services")
                ]);

        IReadOnlyList<string> errors =
            OfficialExpenseRuleSemanticValidator.Validate(
                ReplaceRule(
                    canonical,
                    alteredRule));

        Assert.Contains(
            errors,
            error =>
                error.Contains(
                    MissingSpellcastingRuleId.Value,
                    StringComparison.Ordinal) &&
                error.Contains(
                    "page 159",
                    StringComparison.Ordinal) &&
                error.Contains(
                    "Spellcasting Services",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void DuplicateManagedIdentity_IsRejected()
    {
        IReadOnlyList<RuleDefinition> canonical =
            LoadCanonicalRules();

        RuleDefinition duplicate =
            canonical.Single(
                rule =>
                    rule.Id == MissingSpellcastingRuleId);

        IReadOnlyList<string> errors =
            OfficialExpenseRuleSemanticValidator.Validate(
                canonical
                    .Append(duplicate)
                    .ToArray());

        Assert.Contains(
            errors,
            error =>
                error.Contains(
                    "duplicate ID",
                    StringComparison.Ordinal) &&
                error.Contains(
                    MissingSpellcastingRuleId.Value,
                    StringComparison.Ordinal));
    }

    [Fact]
    public void CompositionRejectsMissingUnreferencedRule()
    {
        IReadOnlyList<RuleDefinition> altered =
            LoadCanonicalRules()
                .Where(
                    rule =>
                        rule.Id != MissingSpellcastingRuleId)
                .ToArray();

        RulesetDefinitionSet definitions =
            CreateDefinitionSet(altered);

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () =>
                    CatalogIntegrityValidator.EnsureValid(
                        definitions));

        Assert.Contains(
            MissingSpellcastingRuleId.Value,
            exception.Message,
            StringComparison.Ordinal);
    }

    private static IReadOnlyList<RuleDefinition> ReplaceRule(
        IReadOnlyList<RuleDefinition> definitions,
        RuleDefinition replacement)
    {
        return definitions
            .Select(
                definition =>
                    definition.Id == replacement.Id
                        ? replacement
                        : definition)
            .ToArray();
    }

    private static IReadOnlyList<RuleDefinition>
        LoadCanonicalRules()
    {
        return RuleDefinitionLoader.LoadAndMergeFromFiles(
            [
                DataPath(Path.Combine("rules", "weapon-rule.json")),
                DataPath(Path.Combine("rules", "armor-rule.json")),
                DataPath(Path.Combine("rules", "adventuring-gear-rule.json")),
                DataPath(Path.Combine("rules", "tool-rule.json")),
                DataPath(Path.Combine("rules", "mount-vehicle-rule.json")),
                DataPath(Path.Combine("rules", "trade-good-rule.json")),
                DataPath(Path.Combine("rules", "expense-rule.json")),
                DataPath(Path.Combine("rules", "lifestyle-rule.json")),
                DataPath(Path.Combine("rules", "race-rule.json")),
                DataPath(Path.Combine("rules", "class-rule.json"))
            ]);
    }

    private static RulesetDefinitionSet CreateDefinitionSet(
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
            mounts:
                MountDefinitionLoader.LoadFromFile(
                    DataPath("mounts.json")),
            vehicles:
                VehicleDefinitionLoader.LoadFromFile(
                    DataPath("vehicles.json")),
            mountSupport:
                MountSupportDefinitionLoader.LoadFromFile(
                    DataPath("mount-support.json")),
            tradeGoods: [],
            mountVehicleRules:
                MountVehicleRulesLoader.LoadFromFile(
                    DataPath("mount-vehicle-rules.json")),
            armorUsage:
                ArmorUsageRulesLoader.LoadFromFile(
                    DataPath("armor-usage.json")));

        var expenses = new ExpenseDefinitionSet(
            lifestyles:
                LifestyleDefinitionLoader.LoadFromFile(
                    DataPath("lifestyles.json")),
            foodAndDrink:
                FoodDrinkDefinitionLoader.LoadFromFile(
                    DataPath("food-drink.json")),
            hospitalityCosts:
                LifestyleHospitalityCostDefinitionLoader
                    .LoadFromFile(
                        DataPath(
                            "lifestyle-hospitality-costs.json")),
            mundaneServices:
                MundaneServiceDefinitionLoader.LoadFromFile(
                    DataPath("mundane-services.json")));

        return new RulesetDefinitionSet(
            sourceDocuments:
                SourceDocumentLoader.LoadFromFile(
                    DataPath("sources.json")),
            rules: rules,
            equipment: equipment,
            expenses: expenses,
            creatureVocabulary:
                new CreatureVocabularyDefinitionSet(
                    abilities:
                        AbilityDefinitionLoader.LoadFromFile(
                            DataPath("abilities.json")),
                    skills:
                        SkillDefinitionLoader.LoadFromFile(
                            DataPath("skills.json")),
                    languages:
                        LanguageDefinitionLoader.LoadFromFile(
                            DataPath("languages.json")),
                    sizes:
                        CreatureSizeDefinitionLoader.LoadFromFile(
                            DataPath("creature-sizes.json")),
                    conditions:
                        ConditionDefinitionLoader.LoadFromFile(
                            DataPath("conditions.json")),
                    damageTypes:
                        DamageTypeDefinitionLoader.LoadFromFile(
                            DataPath("damage-types.json")),
                    senses:
                        SenseDefinitionLoader.LoadFromFile(
                            DataPath("senses.json")),
                    alignments:
                        AlignmentDefinitionLoader.LoadFromFile(
                            DataPath("alignments.json"))),
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

    private static string DataPath(string fileName)
    {
        return Path.Combine(
            FindRepositoryRoot(),
            "Data",
            "dnd5e2014",
            fileName);
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory =
            new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(
                    Path.Combine(
                        directory.FullName,
                        "FiveEData.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not locate the FiveEData repository root.");
    }
}
