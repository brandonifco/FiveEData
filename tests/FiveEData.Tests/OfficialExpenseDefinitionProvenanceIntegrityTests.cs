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
using FiveEData.Rules.Expenses.FoodAndLodging;
using FiveEData.Rules.Expenses.FoodAndLodging.Serialization;
using FiveEData.Rules.Expenses.Lifestyles;
using FiveEData.Rules.Expenses.Lifestyles.Serialization;
using FiveEData.Rules.Expenses.Services;
using FiveEData.Rules.Expenses.Services.Serialization;

namespace FiveEData.Tests;

public sealed class
    OfficialExpenseDefinitionProvenanceIntegrityTests
{
    private static readonly SourceDocumentId OfficialSourceId =
        new("dnd5e2014.source.phb-first-printing");

    [Fact]
    public void CanonicalDefinitions_HaveNoErrors()
    {
        Assert.Empty(
            Validate(LoadCanonicalExpenses()));
    }

    [Fact]
    public void WrongDocument_IsRejected()
    {
        ExpenseDefinitionSet canonical =
            LoadCanonicalExpenses();

        LifestyleDefinition modest =
            canonical.Lifestyles.Single(
                definition =>
                    definition.Id.Value ==
                    "dnd5e2014.lifestyle.modest");

        var alteredModest =
            new LifestyleDefinition(
                modest.Id,
                modest.Name,
                modest.DailyCost,
                modest.SpecialRuleIds,
                [
                    new SourceReference(
                        new SourceDocumentId(
                            "dnd5e2014.source.wrong"),
                        page: 157,
                        section:
                            "Chapter 5: Equipment — Expenses — " +
                            "Lifestyle Expenses")
                ]);

        IReadOnlyList<string> errors =
            Validate(
                new ExpenseDefinitionSet(
                    Replace(
                        canonical.Lifestyles,
                        modest.Id,
                        alteredModest),
                    canonical.FoodAndDrink,
                    canonical.HospitalityCosts,
                    canonical.MundaneServices));

        Assert.Contains(
            errors,
            error =>
                error.Contains(
                    modest.Id.Value,
                    StringComparison.Ordinal) &&
                error.Contains(
                    "dnd5e2014.source.wrong",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void WrongPage_IsRejected()
    {
        ExpenseDefinitionSet altered =
            AlterBreadSource(
                LoadCanonicalExpenses(),
                page: 157,
                section:
                    "Chapter 5: Equipment — Expenses — " +
                    "Food, Drink, and Lodging");

        IReadOnlyList<string> errors = Validate(altered);

        Assert.Contains(
            errors,
            error =>
                error.Contains(
                    "dnd5e2014.food-drink.bread",
                    StringComparison.Ordinal) &&
                error.Contains(
                    "page 158",
                    StringComparison.Ordinal) &&
                error.Contains(
                    "page 157",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void WrongSection_IsRejected()
    {
        ExpenseDefinitionSet canonical =
            LoadCanonicalExpenses();

        LifestyleHospitalityCostDefinition modest =
            canonical.HospitalityCosts.Single(
                definition =>
                    definition.LifestyleId.Value ==
                    "dnd5e2014.lifestyle.modest");

        var alteredModest =
            new LifestyleHospitalityCostDefinition(
                modest.LifestyleId,
                modest.InnStayCostPerDay,
                modest.MealsCostPerDay,
                modest.SpecialRuleIds,
                [
                    new SourceReference(
                        OfficialSourceId,
                        page: 158,
                        section:
                            "Chapter 5: Equipment — Expenses — " +
                            "Services")
                ]);

        IReadOnlyList<string> errors =
            Validate(
                new ExpenseDefinitionSet(
                    canonical.Lifestyles,
                    canonical.FoodAndDrink,
                    Replace(
                        canonical.HospitalityCosts,
                        modest.LifestyleId,
                        alteredModest),
                    canonical.MundaneServices));

        Assert.Contains(
            errors,
            error =>
                error.Contains(
                    modest.LifestyleId.Value,
                    StringComparison.Ordinal) &&
                error.Contains(
                    "Food, Drink, and Lodging",
                    StringComparison.Ordinal) &&
                error.Contains(
                    "Services",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void MissingSource_IsRejected()
    {
        ExpenseDefinitionSet canonical =
            LoadCanonicalExpenses();

        MundaneServiceDefinition messenger =
            canonical.MundaneServices.Single(
                definition =>
                    definition.Id.Value ==
                    "dnd5e2014.mundane-service.messenger");

        var alteredMessenger =
            new MundaneServiceDefinition(
                messenger.Id,
                messenger.Name,
                messenger.Cost,
                messenger.PricingUnit,
                messenger.SpecialRuleIds,
                sources: []);

        IReadOnlyList<string> errors =
            Validate(
                new ExpenseDefinitionSet(
                    canonical.Lifestyles,
                    canonical.FoodAndDrink,
                    canonical.HospitalityCosts,
                    Replace(
                        canonical.MundaneServices,
                        messenger.Id,
                        alteredMessenger)));

        Assert.Contains(
            errors,
            error =>
                error.Contains(
                    messenger.Id.Value,
                    StringComparison.Ordinal) &&
                error.Contains(
                    "exactly one source reference; found 0",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void AdditionalSource_IsRejected()
    {
        ExpenseDefinitionSet canonical =
            LoadCanonicalExpenses();

        LifestyleDefinition poor =
            canonical.Lifestyles.Single(
                definition =>
                    definition.Id.Value ==
                    "dnd5e2014.lifestyle.poor");

        var alteredPoor =
            new LifestyleDefinition(
                poor.Id,
                poor.Name,
                poor.DailyCost,
                poor.SpecialRuleIds,
                poor.Sources
                    .Append(poor.Sources[0])
                    .ToArray());

        IReadOnlyList<string> errors =
            Validate(
                new ExpenseDefinitionSet(
                    Replace(
                        canonical.Lifestyles,
                        poor.Id,
                        alteredPoor),
                    canonical.FoodAndDrink,
                    canonical.HospitalityCosts,
                    canonical.MundaneServices));

        Assert.Contains(
            errors,
            error =>
                error.Contains(
                    poor.Id.Value,
                    StringComparison.Ordinal) &&
                error.Contains(
                    "exactly one source reference; found 2",
                    StringComparison.Ordinal));
    }

    [Fact]
    public void CompositionRejectsWrongExpensePage()
    {
        ExpenseDefinitionSet altered =
            AlterBreadSource(
                LoadCanonicalExpenses(),
                page: 157,
                section:
                    "Chapter 5: Equipment — Expenses — " +
                    "Food, Drink, and Lodging");

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () =>
                    CatalogIntegrityValidator.EnsureValid(
                        CreateDefinitionSet(altered)));

        Assert.Contains(
            "dnd5e2014.food-drink.bread",
            exception.Message,
            StringComparison.Ordinal);

        Assert.Contains(
            "page 158",
            exception.Message,
            StringComparison.Ordinal);
    }

    private static ExpenseDefinitionSet AlterBreadSource(
        ExpenseDefinitionSet canonical,
        int page,
        string section)
    {
        FoodDrinkDefinition bread =
            canonical.FoodAndDrink.Single(
                definition =>
                    definition.Id.Value ==
                    "dnd5e2014.food-drink.bread");

        var alteredBread =
            new FoodDrinkDefinition(
                bread.Id,
                bread.Name,
                bread.Cost,
                bread.PricingUnit,
                bread.SpecialRuleIds,
                [
                    new SourceReference(
                        OfficialSourceId,
                        page,
                        section)
                ]);

        return new ExpenseDefinitionSet(
            canonical.Lifestyles,
            Replace(
                canonical.FoodAndDrink,
                bread.Id,
                alteredBread),
            canonical.HospitalityCosts,
            canonical.MundaneServices);
    }

    private static IReadOnlyList<LifestyleDefinition> Replace(
        IReadOnlyList<LifestyleDefinition> definitions,
        LifestyleId id,
        LifestyleDefinition replacement)
    {
        return definitions
            .Select(
                definition =>
                    definition.Id == id
                        ? replacement
                        : definition)
            .ToArray();
    }

    private static IReadOnlyList<FoodDrinkDefinition> Replace(
        IReadOnlyList<FoodDrinkDefinition> definitions,
        FoodDrinkId id,
        FoodDrinkDefinition replacement)
    {
        return definitions
            .Select(
                definition =>
                    definition.Id == id
                        ? replacement
                        : definition)
            .ToArray();
    }

    private static IReadOnlyList<
        LifestyleHospitalityCostDefinition> Replace(
            IReadOnlyList<
                LifestyleHospitalityCostDefinition> definitions,
            LifestyleId id,
            LifestyleHospitalityCostDefinition replacement)
    {
        return definitions
            .Select(
                definition =>
                    definition.LifestyleId == id
                        ? replacement
                        : definition)
            .ToArray();
    }

    private static IReadOnlyList<MundaneServiceDefinition> Replace(
        IReadOnlyList<MundaneServiceDefinition> definitions,
        MundaneServiceId id,
        MundaneServiceDefinition replacement)
    {
        return definitions
            .Select(
                definition =>
                    definition.Id == id
                        ? replacement
                        : definition)
            .ToArray();
    }

    private static IReadOnlyList<string> Validate(
        ExpenseDefinitionSet expenses)
    {
        return OfficialExpenseSemanticValidator.Validate(
            LoadCanonicalRules(),
            expenses.Lifestyles,
            expenses.FoodAndDrink,
            expenses.HospitalityCosts,
            expenses.MundaneServices);
    }

    private static ExpenseDefinitionSet
        LoadCanonicalExpenses()
    {
        return new ExpenseDefinitionSet(
            LifestyleDefinitionLoader.LoadFromFile(
                DataPath("lifestyles.json")),
            FoodDrinkDefinitionLoader.LoadFromFile(
                DataPath("food-drink.json")),
            LifestyleHospitalityCostDefinitionLoader
                .LoadFromFile(
                    DataPath(
                        "lifestyle-hospitality-costs.json")),
            MundaneServiceDefinitionLoader.LoadFromFile(
                DataPath("mundane-services.json")));
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
        ExpenseDefinitionSet expenses)
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

        return new RulesetDefinitionSet(
            sourceDocuments:
                SourceDocumentLoader.LoadFromFile(
                    DataPath("sources.json")),
            rules: LoadCanonicalRules(),
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
            spellSlotProgressions: [],
            extraAttackProgressions: [],
            backgrounds: []);
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
