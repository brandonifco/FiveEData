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
        Assert.False(typeof(BackgroundDefinitionLoader).IsPublic);
        Assert.False(typeof(BackgroundDefinitionValidator).IsPublic);
        Assert.False(typeof(BackgroundCatalogIntegrityValidator).IsPublic);
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
