using FiveEData.Rules.Common;
using FiveEData.Rules.Equipment.MountSupport;
using FiveEData.Rules.Equipment.MountsAndVehicles;
using FiveEData.Rules.Equipment.MountsAndVehicles.Serialization;
using FiveEData.Rules.Equipment.Vehicles;

namespace FiveEData.Tests;

public sealed class MountVehicleRulesDataFileTests
{
    [Fact]
    public void CanonicalFile_PreservesFirstPrintingRuleFacts()
    {
        MountVehicleRules rules = LoadCanonical();

        Assert.Equal(5, rules.DrawnVehicleCarryingCapacityMultiplier);
        Assert.True(rules.DrawnVehicleCapacityIncludesVehicleWeight);
        Assert.True(rules.MultipleAnimalsCombineCarryingCapacity);

        Assert.True(rules.OtherMountsAreRare);
        Assert.False(rules.OtherMountsNormallyAvailableForPurchase);

        Assert.True(rules.BardingAvailableForAnyArmorType);
        Assert.Equal(4, rules.BardingCostMultiplier);
        Assert.Equal(2, rules.BardingWeightMultiplier);

        Assert.True(
            rules.MilitarySaddleGrantsAdvantageOnChecksToRemainMounted);
        Assert.True(
            rules.ExoticSaddleRequiredForAquaticOrFlyingMounts);

        Assert.Equal(
            new[] { VehicleKind.Land, VehicleKind.Water },
            rules.VehicleProficiencyKinds);
        Assert.True(
            rules.VehicleProficiencyAddsProficiencyBonusToDifficultControlChecks);

        Assert.Equal(3m, rules.TypicalCurrentSpeed.MilesPerHour);
        Assert.True(rules.DownstreamCurrentAddsToVehicleSpeed);
        Assert.False(
            rules.RowedVesselsCanBeRowedAgainstSignificantCurrent);
        Assert.True(
            rules.RowedVesselsCanBePulledUpstreamByDraftAnimals);
        Assert.Equal(
            "dnd5e2014.vehicle.rowboat",
            rules.RowboatVehicleId.Value);
        Assert.Equal(100m, rules.RowboatOverlandWeight.Pounds);

        var source = Assert.Single(rules.Sources);
        Assert.Equal(155, source.Page);
        Assert.Equal(
            "Chapter 5: Equipment — Mounts and Vehicles",
            source.Section);
    }

    [Fact]
    public void CanonicalCatalogs_AssociateRulesAtSourceOwnedBoundaries()
    {
        Dnd5e2014Ruleset ruleset = Dnd5e2014Ruleset.Instance;
        MountVehicleRules rules = ruleset.MountVehicleRules;

        foreach (var mount in ruleset.Mounts.All)
        {
            Assert.Contains(
                rules.DrawnVehiclePullingRuleId,
                mount.SpecialRuleIds);
            Assert.Contains(
                rules.BardingRuleId,
                mount.SpecialRuleIds);
        }

        foreach (var vehicle in ruleset.Vehicles.All)
        {
            Assert.Contains(
                rules.VehicleProficiencyRuleId,
                vehicle.SpecialRuleIds);

            if (vehicle.Kind == VehicleKind.Land)
            {
                Assert.Contains(
                    rules.DrawnVehiclePullingRuleId,
                    vehicle.SpecialRuleIds);
            }
            else
            {
                Assert.DoesNotContain(
                    rules.DrawnVehiclePullingRuleId,
                    vehicle.SpecialRuleIds);
            }
        }

        Assert.Contains(
            rules.RowedVesselsRuleId,
            ruleset.Vehicles.Get(
                new VehicleId(
                    "dnd5e2014.vehicle.keelboat")).SpecialRuleIds);
        Assert.Contains(
            rules.RowedVesselsRuleId,
            ruleset.Vehicles.Get(
                new VehicleId(
                    "dnd5e2014.vehicle.rowboat")).SpecialRuleIds);

        Assert.Contains(
            rules.MilitarySaddleRuleId,
            ruleset.MountSupport.Get(
                new MountSupportId(
                    "dnd5e2014.mount-support.saddle-military"))
                .SpecialRuleIds);
        Assert.Contains(
            rules.ExoticSaddleRuleId,
            ruleset.MountSupport.Get(
                new MountSupportId(
                    "dnd5e2014.mount-support.saddle-exotic"))
                .SpecialRuleIds);
    }

    [Fact]
    public void CanonicalRules_DoNotDuplicateBardingAsMarketplaceArmor()
    {
        Dnd5e2014Ruleset ruleset = Dnd5e2014Ruleset.Instance;

        Assert.DoesNotContain(
            ruleset.MountSupport.All,
            item => string.Equals(
                item.Name,
                "Barding",
                StringComparison.Ordinal));

        Assert.Equal(12, ruleset.Armor.Count);
        Assert.Equal(4, ruleset.MountVehicleRules.BardingCostMultiplier);
        Assert.Equal(2, ruleset.MountVehicleRules.BardingWeightMultiplier);
    }

    private static MountVehicleRules LoadCanonical()
    {
        return MountVehicleRulesLoader.LoadFromFile(
            Path.Combine(
                FindRepositoryRoot(),
                "Data",
                "dnd5e2014",
                "mount-vehicle-rules.json"));
    }

    private static string FindRepositoryRoot()
    {
        DirectoryInfo? directory =
            new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            if (File.Exists(
                    Path.Combine(directory.FullName, "FiveEData.sln")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not locate the FiveEData repository root.");
    }
}
