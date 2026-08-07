using FiveEData.Rules.Backgrounds;
using FiveEData.Rules.Spells;
using FiveEData.Rules.Spells.MagicSchools;
using FiveEData.Rules.Spells.Serialization;
using FiveEData.Rules.Spells.MagicSchools.Serialization;
using FiveEData.Rules.Backgrounds.Serialization;
using FiveEData.Rules.Classes;
using FiveEData.Rules.Classes.Serialization;
using FiveEData.Rules.Classes.BattleMasterManeuvers;
using FiveEData.Rules.Classes.BattleMasterManeuvers.Serialization;
using FiveEData.Rules.Classes.ChannelDivinityOptions;
using FiveEData.Rules.Classes.ChannelDivinityOptions.Serialization;
using FiveEData.Rules.Classes.EldritchInvocations;
using FiveEData.Rules.Classes.EldritchInvocations.Serialization;
using FiveEData.Rules.Classes.ElementalDisciplines;
using FiveEData.Rules.Classes.ElementalDisciplines.Serialization;
using FiveEData.Rules.Classes.FightingStyles;
using FiveEData.Rules.Classes.FightingStyles.Serialization;
using FiveEData.Rules.Classes.Metamagic;
using FiveEData.Rules.Classes.Metamagic.Serialization;
using FiveEData.Rules.Classes.Spellcasting;
using FiveEData.Rules.Classes.Spellcasting.Serialization;
using FiveEData.Rules.Classes.ExtraAttack;
using FiveEData.Rules.Classes.ExtraAttack.Serialization;
using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Creatures;
using FiveEData.Rules.Creatures.Abilities;
using FiveEData.Rules.Creatures.Abilities.Serialization;
using FiveEData.Rules.Creatures.Alignments;
using FiveEData.Rules.Creatures.Alignments.Serialization;
using FiveEData.Rules.Creatures.Conditions;
using FiveEData.Rules.Creatures.Conditions.Serialization;
using FiveEData.Rules.Creatures.DamageTypes;
using FiveEData.Rules.Creatures.DamageTypes.Serialization;
using FiveEData.Rules.Creatures.Languages;
using FiveEData.Rules.Creatures.Languages.Serialization;
using FiveEData.Rules.Creatures.Races;
using FiveEData.Rules.Creatures.Races.Serialization;
using FiveEData.Rules.Creatures.Senses;
using FiveEData.Rules.Creatures.Senses.Serialization;
using FiveEData.Rules.Creatures.Sizes;
using FiveEData.Rules.Creatures.Sizes.Serialization;
using FiveEData.Rules.Creatures.Skills;
using FiveEData.Rules.Creatures.Skills.Serialization;
using FiveEData.Rules.Equipment.Ammunition;
using FiveEData.Rules.Equipment.Ammunition.Serialization;
using FiveEData.Rules.Equipment.AdventuringGear;
using FiveEData.Rules.Equipment.AdventuringGear.Serialization;
using FiveEData.Rules.Equipment.Armor;
using FiveEData.Rules.Equipment.Armor.Serialization;
using FiveEData.Rules.Equipment.Shields;
using FiveEData.Rules.Equipment.Shields.Serialization;
using FiveEData.Rules.Equipment.Mounts;
using FiveEData.Rules.Equipment.Mounts.Serialization;
using FiveEData.Rules.Equipment.MountSupport;
using FiveEData.Rules.Equipment.MountSupport.Serialization;
using FiveEData.Rules.Equipment.MountsAndVehicles;
using FiveEData.Rules.Equipment.MountsAndVehicles.Serialization;
using FiveEData.Rules.Equipment.TradeGoods;
using FiveEData.Rules.Equipment.TradeGoods.Serialization;
using FiveEData.Rules.Equipment.Vehicles;
using FiveEData.Rules.Equipment.Vehicles.Serialization;
using FiveEData.Rules.Equipment.Tools;
using FiveEData.Rules.Equipment.Tools.Serialization;
using FiveEData.Rules.Equipment.Weapons;
using FiveEData.Rules.Equipment.Weapons.Serialization;
using FiveEData.Rules.Expenses;
using FiveEData.Rules.Expenses.FoodAndLodging;
using FiveEData.Rules.Expenses.FoodAndLodging.Serialization;
using FiveEData.Rules.Expenses.Lifestyles;
using FiveEData.Rules.Expenses.Lifestyles.Serialization;
using FiveEData.Rules.Expenses.Services;
using FiveEData.Rules.Expenses.Services.Serialization;

namespace FiveEData.Rules.Catalog;

internal static class Dnd5e2014RulesetLoader
{
    private const string SourcesResource =
        "FiveEData.Data.dnd5e2014.sources.json";

    private const string AbilitiesResource =
        "FiveEData.Data.dnd5e2014.abilities.json";

    private const string SkillsResource =
        "FiveEData.Data.dnd5e2014.skills.json";

    private const string LanguagesResource =
        "FiveEData.Data.dnd5e2014.languages.json";

    private const string CreatureSizesResource =
        "FiveEData.Data.dnd5e2014.creature-sizes.json";

    private const string ConditionsResource =
        "FiveEData.Data.dnd5e2014.conditions.json";

    private const string DamageTypesResource =
        "FiveEData.Data.dnd5e2014.damage-types.json";

    private const string SensesResource =
        "FiveEData.Data.dnd5e2014.senses.json";

    private const string AlignmentsResource =
        "FiveEData.Data.dnd5e2014.alignments.json";

    private const string RacesResource =
        "FiveEData.Data.dnd5e2014.races.json";

    private const string SubracesResource =
        "FiveEData.Data.dnd5e2014.subraces.json";

    private const string ClassesResource =
        "FiveEData.Data.dnd5e2014.classes.json";

    private const string SubclassesResource =
        "FiveEData.Data.dnd5e2014.subclasses.json";

    private const string FightingStylesResource =
        "FiveEData.Data.dnd5e2014.fighting-styles.json";

    private const string MetamagicOptionsResource =
        "FiveEData.Data.dnd5e2014.metamagic-options.json";

    private const string MagicSchoolsResource =
        "FiveEData.Data.dnd5e2014.magic-schools.json";

    private const string SpellsResource =
        "FiveEData.Data.dnd5e2014.spells.json";

    private const string BattleMasterManeuversResource =
        "FiveEData.Data.dnd5e2014.battle-master-maneuvers.json";

    private const string EldritchInvocationsResource =
        "FiveEData.Data.dnd5e2014.eldritch-invocations.json";

    private const string ElementalDisciplinesResource =
        "FiveEData.Data.dnd5e2014.elemental-disciplines.json";

    private const string ChannelDivinityOptionsResource =
        "FiveEData.Data.dnd5e2014.channel-divinity-options.json";

    private const string SpellSlotProgressionsResource =
        "FiveEData.Data.dnd5e2014.spell-slot-progressions.json";

    private const string ExtraAttackProgressionsResource =
        "FiveEData.Data.dnd5e2014.extra-attack-progressions.json";

    private const string BackgroundsResource =
        "FiveEData.Data.dnd5e2014.backgrounds.json";

    private const string AmmunitionResource =
        "FiveEData.Data.dnd5e2014.ammunition.json";

    private const string WeaponsResource =
        "FiveEData.Data.dnd5e2014.weapons.json";

    private const string ArmorResource =
        "FiveEData.Data.dnd5e2014.armor.json";

    private const string ArmorUsageResource =
        "FiveEData.Data.dnd5e2014.armor-usage.json";

    private const string ShieldsResource =
        "FiveEData.Data.dnd5e2014.shields.json";

    private const string AdventuringGearResource =
        "FiveEData.Data.dnd5e2014.adventuring-gear.json";

    private const string ContainerCapacitiesResource =
        "FiveEData.Data.dnd5e2014.container-capacities.json";

    private const string ToolFamiliesResource =
        "FiveEData.Data.dnd5e2014.tool-families.json";

    private const string ToolsResource =
        "FiveEData.Data.dnd5e2014.tools.json";

    private const string MountsResource =
        "FiveEData.Data.dnd5e2014.mounts.json";

    private const string VehiclesResource =
        "FiveEData.Data.dnd5e2014.vehicles.json";

    private const string MountSupportResource =
        "FiveEData.Data.dnd5e2014.mount-support.json";

    private const string MountVehicleRulesResource =
        "FiveEData.Data.dnd5e2014.mount-vehicle-rules.json";

    private const string TradeGoodsResource =
        "FiveEData.Data.dnd5e2014.trade-goods.json";

    private const string LifestylesResource =
        "FiveEData.Data.dnd5e2014.lifestyles.json";

    private const string FoodDrinkResource =
        "FiveEData.Data.dnd5e2014.food-drink.json";

    private const string HospitalityCostsResource =
        "FiveEData.Data.dnd5e2014.lifestyle-hospitality-costs.json";

    private const string MundaneServicesResource =
        "FiveEData.Data.dnd5e2014.mundane-services.json";

    private const string WeaponRuleResource =
        "FiveEData.Data.dnd5e2014.rules.weapon-rule.json";

    private const string ArmorRuleResource =
        "FiveEData.Data.dnd5e2014.rules.armor-rule.json";

    private const string AdventuringGearRuleResource =
        "FiveEData.Data.dnd5e2014.rules.adventuring-gear-rule.json";

    private const string ToolRuleResource =
        "FiveEData.Data.dnd5e2014.rules.tool-rule.json";

    private const string MountVehicleRuleResource =
        "FiveEData.Data.dnd5e2014.rules.mount-vehicle-rule.json";

    private const string TradeGoodRuleResource =
        "FiveEData.Data.dnd5e2014.rules.trade-good-rule.json";

    private const string ExpenseRuleResource =
        "FiveEData.Data.dnd5e2014.rules.expense-rule.json";

    private const string LifestyleRuleResource =
        "FiveEData.Data.dnd5e2014.rules.lifestyle-rule.json";

    private const string RaceRuleResource =
        "FiveEData.Data.dnd5e2014.rules.race-rule.json";

    private const string ClassRuleResource =
        "FiveEData.Data.dnd5e2014.rules.class-rule.json";

    private const string BackgroundRuleResource =
        "FiveEData.Data.dnd5e2014.rules.background-rule.json";

    public static Dnd5e2014Ruleset Load()
    {
        IReadOnlyList<SourceDocument> sources =
            SourceDocumentLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(SourcesResource));

        IReadOnlyList<AbilityDefinition> abilities =
            AbilityDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(
                    AbilitiesResource));

        IReadOnlyList<SkillDefinition> skills =
            SkillDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(
                    SkillsResource));

        IReadOnlyList<LanguageDefinition> languages =
            LanguageDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(
                    LanguagesResource));

        IReadOnlyList<CreatureSizeDefinition> sizes =
            CreatureSizeDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(
                    CreatureSizesResource));

        IReadOnlyList<ConditionDefinition> conditions =
            ConditionDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(
                    ConditionsResource));

        IReadOnlyList<DamageTypeDefinition> damageTypes =
            DamageTypeDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(
                    DamageTypesResource));

        IReadOnlyList<SenseDefinition> senses =
            SenseDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(
                    SensesResource));

        IReadOnlyList<AlignmentDefinition> alignments =
            AlignmentDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(
                    AlignmentsResource));

        IReadOnlyList<RaceDefinition> races =
            RaceDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(RacesResource));

        IReadOnlyList<SubraceDefinition> subraces =
            SubraceDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(SubracesResource));

        IReadOnlyList<ClassDefinition> classes =
            ClassDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(ClassesResource));

        IReadOnlyList<SubclassDefinition> subclasses =
            SubclassDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(SubclassesResource));

        IReadOnlyList<FightingStyleDefinition> fightingStyles =
            FightingStyleDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(FightingStylesResource));

        IReadOnlyList<MetamagicOptionDefinition> metamagicOptions =
            MetamagicOptionDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(
                    MetamagicOptionsResource));

        IReadOnlyList<MagicSchoolDefinition> magicSchools =
            MagicSchoolDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(
                    MagicSchoolsResource));

        IReadOnlyList<SpellDefinition> spells =
            SpellDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(SpellsResource));

        IReadOnlyList<BattleMasterManeuverDefinition> battleMasterManeuvers =
            BattleMasterManeuverDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(
                    BattleMasterManeuversResource));

        IReadOnlyList<EldritchInvocationDefinition> eldritchInvocations =
            EldritchInvocationDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(
                    EldritchInvocationsResource));

        IReadOnlyList<ElementalDisciplineDefinition> elementalDisciplines =
            ElementalDisciplineDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(
                    ElementalDisciplinesResource));

        IReadOnlyList<ChannelDivinityOptionDefinition>
            channelDivinityOptions =
                ChannelDivinityOptionDefinitionLoader.LoadFromJson(
                    EmbeddedDataReader.ReadRequiredText(
                        ChannelDivinityOptionsResource));

        IReadOnlyList<SpellSlotProgressionDefinition> spellSlotProgressions =
            SpellSlotProgressionDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(
                    SpellSlotProgressionsResource));

        IReadOnlyList<ExtraAttackProgressionDefinition>
            extraAttackProgressions =
                ExtraAttackProgressionDefinitionLoader.LoadFromJson(
                    EmbeddedDataReader.ReadRequiredText(
                        ExtraAttackProgressionsResource));

        IReadOnlyList<BackgroundDefinition> backgrounds =
            BackgroundDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(BackgroundsResource));

        IReadOnlyList<AmmunitionDefinition> ammunition =
            AmmunitionDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(AmmunitionResource));

        IReadOnlyList<WeaponDefinition> weapons =
            WeaponDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(WeaponsResource));

        IReadOnlyList<ArmorDefinition> armor =
            ArmorDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(ArmorResource));

        ArmorUsageRules armorUsage =
            ArmorUsageRulesLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(ArmorUsageResource));

        IReadOnlyList<ShieldDefinition> shields =
            ShieldDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(ShieldsResource));

        IReadOnlyList<AdventuringGearDefinition> adventuringGear =
            AdventuringGearDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(AdventuringGearResource));

        IReadOnlyList<ContainerCapacityDefinition> containerCapacities =
            ContainerCapacityDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(ContainerCapacitiesResource));

        IReadOnlyList<ToolFamilyDefinition> toolFamilies =
            ToolFamilyDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(ToolFamiliesResource));

        IReadOnlyList<ToolDefinition> tools =
            ToolDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(ToolsResource));

        IReadOnlyList<MountDefinition> mounts =
            MountDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(MountsResource));

        IReadOnlyList<VehicleDefinition> vehicles =
            VehicleDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(VehiclesResource));

        IReadOnlyList<MountSupportDefinition> mountSupport =
            MountSupportDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(MountSupportResource));

        MountVehicleRules mountVehicleRules =
            MountVehicleRulesLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(MountVehicleRulesResource));

        IReadOnlyList<TradeGoodDefinition> tradeGoods =
            TradeGoodDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(TradeGoodsResource));

        IReadOnlyList<LifestyleDefinition> lifestyles =
            LifestyleDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(LifestylesResource));

        IReadOnlyList<FoodDrinkDefinition> foodAndDrink =
            FoodDrinkDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(FoodDrinkResource));

        IReadOnlyList<LifestyleHospitalityCostDefinition>
            hospitalityCosts =
                LifestyleHospitalityCostDefinitionLoader.LoadFromJson(
                    EmbeddedDataReader.ReadRequiredText(
                        HospitalityCostsResource));

        IReadOnlyList<MundaneServiceDefinition> mundaneServices =
            MundaneServiceDefinitionLoader.LoadFromJson(
                EmbeddedDataReader.ReadRequiredText(
                    MundaneServicesResource));

        IReadOnlyList<RuleDefinition> rules =
            RuleDefinitionLoader.LoadAndMergeFromJson(
                [
                    EmbeddedDataReader.ReadRequiredText(WeaponRuleResource),
                    EmbeddedDataReader.ReadRequiredText(ArmorRuleResource),
                    EmbeddedDataReader.ReadRequiredText(
                        AdventuringGearRuleResource),
                    EmbeddedDataReader.ReadRequiredText(ToolRuleResource),
                    EmbeddedDataReader.ReadRequiredText(
                        MountVehicleRuleResource),
                    EmbeddedDataReader.ReadRequiredText(TradeGoodRuleResource),
                    EmbeddedDataReader.ReadRequiredText(ExpenseRuleResource),
                    EmbeddedDataReader.ReadRequiredText(LifestyleRuleResource),
                    EmbeddedDataReader.ReadRequiredText(RaceRuleResource),
                    EmbeddedDataReader.ReadRequiredText(ClassRuleResource),
                    EmbeddedDataReader.ReadRequiredText(BackgroundRuleResource)
                ]);

        var equipment = new EquipmentDefinitionSet(
            weapons: weapons,
            ammunition: ammunition,
            armor: armor,
            shields: shields,
            adventuringGear: adventuringGear,
            containerCapacities: containerCapacities,
            toolFamilies: toolFamilies,
            tools: tools,
            mounts: mounts,
            vehicles: vehicles,
            mountSupport: mountSupport,
            tradeGoods: tradeGoods,
            mountVehicleRules: mountVehicleRules,
            armorUsage: armorUsage);

        var expenses = new ExpenseDefinitionSet(
            lifestyles: lifestyles,
            foodAndDrink: foodAndDrink,
            hospitalityCosts: hospitalityCosts,
            mundaneServices: mundaneServices);

        var creatureVocabulary =
            new CreatureVocabularyDefinitionSet(
                abilities: abilities,
                skills: skills,
                languages: languages,
                sizes: sizes,
                conditions: conditions,
                damageTypes: damageTypes,
                senses: senses,
                alignments: alignments);

        var raceDefinitionSet = new RaceDefinitionSet(
            races: races,
            subraces: subraces);

        var classDefinitionSet = new ClassDefinitionSet(
            classes: classes,
            subclasses: subclasses);

        var definitions = new RulesetDefinitionSet(
            sourceDocuments: sources,
            rules: rules,
            equipment: equipment,
            expenses: expenses,
            creatureVocabulary: creatureVocabulary,
            races: raceDefinitionSet,
            classes: classDefinitionSet,
            fightingStyles: fightingStyles,
            metamagicOptions: metamagicOptions,
            magicSchools: magicSchools,
            spells: spells,
            battleMasterManeuvers: battleMasterManeuvers,
            eldritchInvocations: eldritchInvocations,
            elementalDisciplines: elementalDisciplines,
            channelDivinityOptions: channelDivinityOptions,
            spellSlotProgressions: spellSlotProgressions,
            extraAttackProgressions: extraAttackProgressions,
            backgrounds: backgrounds);

        CatalogIntegrityValidator.EnsureValid(definitions);

        return new Dnd5e2014Ruleset(
            weapons: new WeaponCatalog(weapons),
            ammunition: new AmmunitionCatalog(ammunition),
            armor: new ArmorCatalog(armor),
            shields: new ShieldCatalog(shields),
            adventuringGear: new AdventuringGearCatalog(adventuringGear),
            containerCapacities: new ContainerCapacityCatalog(containerCapacities),
            toolFamilies: new ToolFamilyCatalog(toolFamilies),
            tools: new ToolCatalog(tools),
            mounts: new MountCatalog(mounts),
            vehicles: new VehicleCatalog(vehicles),
            mountSupport: new MountSupportCatalog(mountSupport),
            mountVehicleRules: mountVehicleRules,
            tradeGoods: new TradeGoodCatalog(tradeGoods),
            armorUsage: armorUsage,
            expenses: new ExpenseCatalogs(
                lifestyles: new LifestyleCatalog(lifestyles),
                foodAndDrink: new FoodDrinkCatalog(foodAndDrink),
                hospitalityCosts:
                    new LifestyleHospitalityCostCatalog(
                        hospitalityCosts),
                mundaneServices:
                    new MundaneServiceCatalog(
                        mundaneServices)),
            creatureVocabulary:
                new CreatureVocabularyCatalogs(
                    abilities: new AbilityCatalog(abilities),
                    skills: new SkillCatalog(skills),
                    languages: new LanguageCatalog(languages),
                    sizes: new CreatureSizeCatalog(sizes),
                    conditions: new ConditionCatalog(conditions),
                    damageTypes: new DamageTypeCatalog(damageTypes),
                    senses: new SenseCatalog(senses),
                    alignments: new AlignmentCatalog(alignments)),
            races: new RaceCatalog(races),
            subraces: new SubraceCatalog(subraces),
            classes: new ClassCatalog(classes),
            subclasses: new SubclassCatalog(subclasses),
            fightingStyles: new FightingStyleCatalog(fightingStyles),
            metamagicOptions: new MetamagicOptionCatalog(metamagicOptions),
            magicSchools: new MagicSchoolCatalog(magicSchools),
            spells: new SpellCatalog(spells),
            battleMasterManeuvers:
                new BattleMasterManeuverCatalog(battleMasterManeuvers),
            eldritchInvocations:
                new EldritchInvocationCatalog(eldritchInvocations),
            elementalDisciplines:
                new ElementalDisciplineCatalog(elementalDisciplines),
            channelDivinityOptions:
                new ChannelDivinityOptionCatalog(channelDivinityOptions),
            spellSlotProgressions:
                new SpellSlotProgressionCatalog(spellSlotProgressions),
            extraAttackProgressions:
                new ExtraAttackProgressionCatalog(extraAttackProgressions),
            backgrounds: new BackgroundCatalog(backgrounds),
            sources: new SourceDocumentCatalog(sources),
            rules: new RuleCatalog(rules));
    }
}
