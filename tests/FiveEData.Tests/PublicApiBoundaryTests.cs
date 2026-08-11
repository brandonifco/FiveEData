using System.Reflection;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Catalog;
using FiveEData.Rules.Expenses;
using FiveEData.Rules.Expenses.Lifestyles;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Creatures;
using FiveEData.Rules.Creatures.Abilities.Serialization;
using FiveEData.Rules.Creatures.Alignments.Serialization;
using FiveEData.Rules.Creatures.Conditions.Serialization;
using FiveEData.Rules.Creatures.DamageTypes.Serialization;
using FiveEData.Rules.Backgrounds;
using FiveEData.Rules.Backgrounds.Serialization;
using FiveEData.Rules.Classes;
using FiveEData.Rules.Classes.Serialization;
using FiveEData.Rules.Classes.FightingStyles;
using FiveEData.Rules.Classes.FightingStyles.Serialization;
using FiveEData.Rules.Classes.Spellcasting;
using FiveEData.Rules.Classes.Spellcasting.Serialization;
using FiveEData.Rules.Classes.ExtraAttack;
using FiveEData.Rules.Classes.ExtraAttack.Serialization;
using FiveEData.Rules.Creatures.Languages.Serialization;
using FiveEData.Rules.Creatures.Races;
using FiveEData.Rules.Creatures.Races.Serialization;
using FiveEData.Rules.Creatures.Senses.Serialization;
using FiveEData.Rules.Creatures.Sizes.Serialization;
using FiveEData.Rules.Creatures.Skills.Serialization;
using FiveEData.Rules.Expenses.FoodAndLodging.Serialization;
using FiveEData.Rules.Expenses.Lifestyles.Serialization;
using FiveEData.Rules.Expenses.Services;
using FiveEData.Rules.Expenses.Services.Serialization;
using FiveEData.Rules.Equipment.Ammunition.Serialization;
using FiveEData.Rules.Equipment.AdventuringGear.Serialization;
using FiveEData.Rules.Equipment.Armor.Serialization;
using FiveEData.Rules.Equipment.Mounts.Serialization;
using FiveEData.Rules.Equipment.MountSupport.Serialization;
using FiveEData.Rules.Equipment.MountsAndVehicles.Serialization;
using FiveEData.Rules.Equipment.TradeGoods.Serialization;
using FiveEData.Rules.Equipment.Vehicles.Serialization;
using FiveEData.Rules.Equipment.Shields.Serialization;
using FiveEData.Rules.Equipment.Weapons.Serialization;
using FiveEData.Rules.Equipment.Tools.Serialization;
using FiveEData.Rules.Spells;
using FiveEData.Rules.Spells.Serialization;
using FiveEData.Rules.Spells.MagicSchools;
using FiveEData.Rules.Spells.MagicSchools.Serialization;
using FiveEData.Rules.Combat.CombatActions;
using FiveEData.Rules.Combat.CombatActions.Serialization;
using FiveEData.Rules.Combat.Cover;
using FiveEData.Rules.Combat.Cover.Serialization;
using FiveEData.Rules.Adventuring.TravelPace;
using FiveEData.Rules.Adventuring.TravelPace.Serialization;
using FiveEData.Rules.Adventuring.Resting;
using FiveEData.Rules.Adventuring.Resting.Serialization;
using FiveEData.Rules.Adventuring.DowntimeActivities;
using FiveEData.Rules.Adventuring.DowntimeActivities.Serialization;
using FiveEData.Rules.Classes.Metamagic;
using FiveEData.Rules.Classes.Metamagic.Serialization;
using FiveEData.Rules.Classes.BattleMasterManeuvers;
using FiveEData.Rules.Classes.BattleMasterManeuvers.Serialization;
using FiveEData.Rules.Classes.EldritchInvocations;
using FiveEData.Rules.Classes.EldritchInvocations.Serialization;
using FiveEData.Rules.Classes.ElementalDisciplines;
using FiveEData.Rules.Classes.ElementalDisciplines.Serialization;
using FiveEData.Rules.Classes.HunterOptions;
using FiveEData.Rules.Classes.HunterOptions.Serialization;
using FiveEData.Rules.Characters.CharacterAdvancement;
using FiveEData.Rules.Spells.Concentration;
using FiveEData.Rules.Spells.Concentration.Serialization;
using FiveEData.Rules.Characters.CharacterAdvancement.Serialization;
using FiveEData.Rules.Classes.OpenHandTechniqueOptions;
using FiveEData.Rules.Classes.OpenHandTechniqueOptions.Serialization;
using FiveEData.Rules.Classes.ThirdEyeOptions;
using FiveEData.Rules.Classes.ThirdEyeOptions.Serialization;
using FiveEData.Rules.Classes.TotemWarriorOptions;
using FiveEData.Rules.Classes.TransmutersStoneOptions;
using FiveEData.Rules.Classes.TransmutersStoneOptions.Serialization;
using FiveEData.Rules.Classes.TotemWarriorOptions.Serialization;
using FiveEData.Rules.Classes.ChannelDivinityOptions;
using FiveEData.Rules.Classes.ChannelDivinityOptions.Serialization;

namespace FiveEData.Tests;

public sealed class PublicApiBoundaryTests
{
    [Fact]
    public void SerializationAndIntegrityPlumbing_AreNotPublicApi()
    {
        Assert.False(typeof(AbilityDefinitionLoader).IsPublic);
        Assert.False(typeof(SkillDefinitionLoader).IsPublic);
        Assert.False(typeof(LanguageDefinitionLoader).IsPublic);
        Assert.False(
            typeof(CreatureSizeDefinitionLoader).IsPublic);
        Assert.False(typeof(ConditionDefinitionLoader).IsPublic);
        Assert.False(typeof(DamageTypeDefinitionLoader).IsPublic);
        Assert.False(typeof(SenseDefinitionLoader).IsPublic);
        Assert.False(typeof(AlignmentDefinitionLoader).IsPublic);
        Assert.False(
            typeof(CreatureVocabularyDefinitionSet).IsPublic);
        Assert.False(
            typeof(
                CreatureVocabularyCatalogIntegrityValidator)
                .IsPublic);
        Assert.False(
            typeof(
                OfficialCreatureVocabularySemanticValidator)
                .IsPublic);
        Assert.False(typeof(WeaponDefinitionLoader).IsPublic);
        Assert.False(typeof(AmmunitionDefinitionLoader).IsPublic);
        Assert.False(typeof(AdventuringGearDefinitionLoader).IsPublic);
        Assert.False(typeof(ContainerCapacityDefinitionLoader).IsPublic);
        Assert.False(typeof(ToolDefinitionLoader).IsPublic);
        Assert.False(typeof(ToolFamilyDefinitionLoader).IsPublic);
        Assert.False(typeof(MountDefinitionLoader).IsPublic);
        Assert.False(typeof(MountSupportDefinitionLoader).IsPublic);
        Assert.False(typeof(MountVehicleRulesLoader).IsPublic);
        Assert.False(typeof(TradeGoodDefinitionLoader).IsPublic);
        Assert.False(typeof(FoodDrinkDefinitionLoader).IsPublic);
        Assert.False(typeof(LifestyleHospitalityCostDefinitionLoader).IsPublic);
        Assert.False(typeof(LifestyleDefinitionLoader).IsPublic);
        Assert.False(typeof(LifestyleRuleAssociationIntegrityValidator).IsPublic);
        Assert.False(typeof(OfficialLifestyleSemanticValidator).IsPublic);
        Assert.False(typeof(OfficialExpenseSemanticValidator).IsPublic);
        Assert.False(
            typeof(OfficialExpenseRuleSemanticValidator).IsPublic);
        Assert.False(
            typeof(OfficialSourceExpectation).IsPublic);
        Assert.False(
            typeof(OfficialSourceReferenceSemanticValidator)
                .IsPublic);
        Assert.False(typeof(MundaneServiceDefinitionLoader).IsPublic);
        Assert.False(typeof(MundaneServiceCatalogIntegrityValidator).IsPublic);
        Assert.False(typeof(OfficialMundaneServiceSemanticValidator).IsPublic);
        Assert.False(typeof(VehicleDefinitionLoader).IsPublic);
        Assert.False(typeof(ArmorDefinitionLoader).IsPublic);
        Assert.False(typeof(ArmorUsageRulesLoader).IsPublic);
        Assert.False(typeof(ShieldDefinitionLoader).IsPublic);
        Assert.False(typeof(SourceDocumentLoader).IsPublic);
        Assert.False(typeof(CatalogIntegrityValidator).IsPublic);
        Assert.False(typeof(RuleDefinitionLoader).IsPublic);
        Assert.False(typeof(RaceDefinitionLoader).IsPublic);
        Assert.False(typeof(SubraceDefinitionLoader).IsPublic);
        Assert.False(typeof(RaceDefinitionValidator).IsPublic);
        Assert.False(typeof(SubraceDefinitionValidator).IsPublic);
        Assert.False(typeof(RaceCatalogIntegrityValidator).IsPublic);
        Assert.False(typeof(RaceDefinitionSet).IsPublic);
        Assert.False(typeof(ClassDefinitionLoader).IsPublic);
        Assert.False(typeof(SubclassDefinitionLoader).IsPublic);
        Assert.False(typeof(ClassDefinitionValidator).IsPublic);
        Assert.False(typeof(SubclassDefinitionValidator).IsPublic);
        Assert.False(typeof(ClassCatalogIntegrityValidator).IsPublic);
        Assert.False(typeof(ClassDefinitionSet).IsPublic);
        Assert.False(typeof(FightingStyleDefinitionLoader).IsPublic);
        Assert.False(typeof(FightingStyleDefinitionValidator).IsPublic);
        Assert.False(
            typeof(SpellSlotProgressionDefinitionLoader).IsPublic);
        Assert.False(
            typeof(SpellSlotProgressionDefinitionValidator).IsPublic);
        Assert.False(
            typeof(ExtraAttackProgressionDefinitionLoader).IsPublic);
        Assert.False(
            typeof(ExtraAttackProgressionDefinitionValidator).IsPublic);
        Assert.False(typeof(RulesetDefinitionSet).IsPublic);
        Assert.False(typeof(BackgroundDefinitionLoader).IsPublic);
        Assert.False(typeof(BackgroundDefinitionValidator).IsPublic);
        Assert.False(typeof(BackgroundCatalogIntegrityValidator).IsPublic);
        Assert.False(typeof(MagicSchoolDefinitionLoader).IsPublic);
        Assert.False(typeof(MagicSchoolDefinitionValidator).IsPublic);
        Assert.False(typeof(CombatActionDefinitionLoader).IsPublic);
        Assert.False(typeof(CombatActionDefinitionValidator).IsPublic);
        Assert.False(typeof(CoverDefinitionLoader).IsPublic);
        Assert.False(typeof(CoverDefinitionValidator).IsPublic);
        Assert.False(typeof(TravelPaceDefinitionLoader).IsPublic);
        Assert.False(typeof(TravelPaceDefinitionValidator).IsPublic);
        Assert.False(typeof(RestTypeDefinitionLoader).IsPublic);
        Assert.False(typeof(RestTypeDefinitionValidator).IsPublic);
        Assert.False(typeof(DowntimeActivityDefinitionLoader).IsPublic);
        Assert.False(typeof(DowntimeActivityDefinitionValidator).IsPublic);
        Assert.False(typeof(SpellDefinitionLoader).IsPublic);
        Assert.False(typeof(SpellDefinitionValidator).IsPublic);
        Assert.False(typeof(MetamagicOptionDefinitionLoader).IsPublic);
        Assert.False(typeof(MetamagicOptionDefinitionValidator).IsPublic);
        Assert.False(
            typeof(BattleMasterManeuverDefinitionLoader).IsPublic);
        Assert.False(
            typeof(BattleMasterManeuverDefinitionValidator).IsPublic);
        Assert.False(typeof(EldritchInvocationDefinitionLoader).IsPublic);
        Assert.False(
            typeof(EldritchInvocationDefinitionValidator).IsPublic);
        Assert.False(typeof(ElementalDisciplineDefinitionLoader).IsPublic);
        Assert.False(
            typeof(ElementalDisciplineDefinitionValidator).IsPublic);
        Assert.False(
            typeof(ChannelDivinityOptionDefinitionLoader).IsPublic);
        Assert.False(
            typeof(ChannelDivinityOptionDefinitionValidator).IsPublic);
        Assert.False(typeof(TotemWarriorOptionDefinitionLoader).IsPublic);
        Assert.False(
            typeof(TotemWarriorOptionDefinitionValidator).IsPublic);
        Assert.False(typeof(HunterOptionDefinitionLoader).IsPublic);
        Assert.False(typeof(HunterOptionDefinitionValidator).IsPublic);
        Assert.False(
            typeof(OpenHandTechniqueOptionDefinitionLoader).IsPublic);
        Assert.False(
            typeof(OpenHandTechniqueOptionDefinitionValidator).IsPublic);
        Assert.False(typeof(ThirdEyeOptionDefinitionLoader).IsPublic);
        Assert.False(typeof(ThirdEyeOptionDefinitionValidator).IsPublic);
        Assert.False(
            typeof(TransmutersStoneOptionDefinitionLoader).IsPublic);
        Assert.False(
            typeof(TransmutersStoneOptionDefinitionValidator).IsPublic);
        Assert.False(typeof(CharacterAdvancementRulesLoader).IsPublic);
        Assert.False(typeof(CharacterAdvancementRulesValidator).IsPublic);
        Assert.False(typeof(ConcentrationRulesLoader).IsPublic);
        Assert.False(typeof(ConcentrationRulesValidator).IsPublic);
    }

    [Fact]
    public void ExportedApi_DoesNotExposeFilesystemLoadingMethods()
    {
        Assembly assembly = typeof(Dnd5e2014Ruleset).Assembly;

        MethodInfo[] offending = assembly
            .GetExportedTypes()
            .SelectMany(type => type.GetMethods(
                BindingFlags.Public |
                BindingFlags.Instance |
                BindingFlags.Static |
                BindingFlags.DeclaredOnly))
            .Where(method =>
                method.Name.Contains(
                    "LoadFromFile",
                    StringComparison.Ordinal))
            .ToArray();

        Assert.Empty(offending);
    }

    [Fact]
    public void RulesetCreation_LoadsEmbeddedData()
    {
        Dnd5e2014Ruleset ruleset = Dnd5e2014Ruleset.Instance;

        Assert.Equal(38, ruleset.Weapons.Count);
        Assert.Equal(4, ruleset.Ammunition.Count);
        Assert.Equal(12, ruleset.Armor.Count);
        Assert.Equal(1, ruleset.Shields.Count);
        Assert.Equal(95, ruleset.AdventuringGear.Count);
        Assert.Equal(13, ruleset.ContainerCapacities.Count);
        Assert.Equal(3, ruleset.ToolFamilies.Count);
        Assert.Equal(37, ruleset.Tools.Count);
        Assert.Equal(8, ruleset.Mounts.Count);
        Assert.Equal(11, ruleset.Vehicles.Count);
        Assert.Equal(8, ruleset.MountSupport.Count);
        Assert.Equal(23, ruleset.TradeGoods.Count);
        Assert.Equal(7, ruleset.Expenses.Lifestyles.Count);
        Assert.Equal(8, ruleset.Expenses.FoodAndDrink.Count);
        Assert.Equal(6, ruleset.Expenses.HospitalityCosts.Count);
        Assert.Equal(7, ruleset.Expenses.MundaneServices.Count);
        Assert.Equal(
            16,
            ruleset.CreatureVocabulary.Languages.Count);
        Assert.Equal(
            6,
            ruleset.CreatureVocabulary.Sizes.Count);
        Assert.Equal(
            15,
            ruleset.CreatureVocabulary.Conditions.Count);
        Assert.Equal(
            13,
            ruleset.CreatureVocabulary.DamageTypes.Count);
        Assert.Equal(
            1,
            ruleset.CreatureVocabulary.Senses.Count);
        Assert.Equal(
            9,
            ruleset.CreatureVocabulary.Alignments.Count);
        Assert.Equal(9, ruleset.Races.Count);
        Assert.Equal(9, ruleset.Subraces.Count);
        Assert.Equal(12, ruleset.Classes.Count);
        Assert.Equal(40, ruleset.Subclasses.Count);
        Assert.Equal(13, ruleset.Backgrounds.Count);
        Assert.Equal(445, ruleset.Rules.Count);
        Assert.Equal(1, ruleset.Sources.Count);
    }
}
