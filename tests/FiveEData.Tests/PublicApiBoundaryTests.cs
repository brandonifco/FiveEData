using System.Reflection;
using FiveEData.Rules.Catalog;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Expenses.FoodAndLodging.Serialization;
using FiveEData.Rules.Expenses.Lifestyles.Serialization;
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
        Assert.False(typeof(VehicleDefinitionLoader).IsPublic);
        Assert.False(typeof(ArmorDefinitionLoader).IsPublic);
        Assert.False(typeof(ArmorUsageRulesLoader).IsPublic);
        Assert.False(typeof(ShieldDefinitionLoader).IsPublic);
        Assert.False(typeof(SourceDocumentLoader).IsPublic);
        Assert.False(typeof(CatalogIntegrityValidator).IsPublic);
        Assert.False(typeof(RuleDefinitionLoader).IsPublic);
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
        Assert.Equal(82, ruleset.Rules.Count);
        Assert.Equal(1, ruleset.Sources.Count);
    }
}
