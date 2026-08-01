using FiveEData.Rules.Equipment.MountsAndVehicles;
using FiveEData.Rules.Equipment.MountsAndVehicles.Serialization;
using FiveEData.Rules.Equipment.Vehicles;

namespace FiveEData.Tests;

public sealed class MountVehicleRulesLoaderTests
{
    private const string ValidJson =
        """{"drawnVehiclePullingRuleId":"dnd5e2014.mount-vehicle-rule.drawn-vehicle-pulling-capacity","drawnVehicleCarryingCapacityMultiplier":5,"drawnVehicleCapacityIncludesVehicleWeight":true,"multipleAnimalsCombineCarryingCapacity":true,"otherMountAvailabilityRuleId":"dnd5e2014.mount-vehicle-rule.other-mount-availability","otherMountsAreRare":true,"otherMountsNormallyAvailableForPurchase":false,"bardingRuleId":"dnd5e2014.mount-vehicle-rule.barding","bardingAvailableForAnyArmorType":true,"bardingCostMultiplier":4,"bardingWeightMultiplier":2,"militarySaddleRuleId":"dnd5e2014.mount-vehicle-rule.military-saddle","militarySaddleGrantsAdvantageOnChecksToRemainMounted":true,"exoticSaddleRuleId":"dnd5e2014.mount-vehicle-rule.exotic-saddle","exoticSaddleRequiredForAquaticOrFlyingMounts":true,"vehicleProficiencyRuleId":"dnd5e2014.mount-vehicle-rule.vehicle-proficiency","vehicleProficiencyKinds":["Land","Water"],"vehicleProficiencyAddsProficiencyBonusToDifficultControlChecks":true,"rowedVesselsRuleId":"dnd5e2014.mount-vehicle-rule.rowed-vessels","typicalCurrentSpeedMilesPerHour":3,"downstreamCurrentAddsToVehicleSpeed":true,"rowedVesselsCanBeRowedAgainstSignificantCurrent":false,"rowedVesselsCanBePulledUpstreamByDraftAnimals":true,"rowboatVehicleId":"dnd5e2014.vehicle.rowboat","rowboatOverlandWeight":{"pounds":100},"sources":[{"documentId":"dnd5e2014.source.phb-first-printing","page":155,"section":"Chapter 5: Equipment — Mounts and Vehicles"}]}""";

    [Fact]
    public void ValidDefinition_LoadsStrictly()
    {
        MountVehicleRules rules =
            MountVehicleRulesLoader.LoadFromJson(ValidJson);

        Assert.Equal(5, rules.DrawnVehicleCarryingCapacityMultiplier);
        Assert.Equal(4, rules.BardingCostMultiplier);
        Assert.Equal(2, rules.BardingWeightMultiplier);
        Assert.Equal(3m, rules.TypicalCurrentSpeed.MilesPerHour);
        Assert.Equal(100m, rules.RowboatOverlandWeight.Pounds);
        Assert.Equal(
            new[] { VehicleKind.Land, VehicleKind.Water },
            rules.VehicleProficiencyKinds);
    }

    [Fact]
    public void UnknownProperty_IsRejected()
    {
        string json = ValidJson.Replace(
            "\"sources\":[",
            "\"unexpected\":true,\"sources\":[");

        Assert.Throws<InvalidDataException>(
            () => MountVehicleRulesLoader.LoadFromJson(json));
    }

    [Fact]
    public void MissingVehicleProficiencyKinds_IsRejected()
    {
        string json = ValidJson.Replace(
            "\"vehicleProficiencyKinds\":[\"Land\",\"Water\"],",
            string.Empty);

        Assert.Throws<InvalidDataException>(
            () => MountVehicleRulesLoader.LoadFromJson(json));
    }

    [Fact]
    public void UnknownVehicleKind_IsRejected()
    {
        string json = ValidJson.Replace(
            "[\"Land\",\"Water\"]",
            "[\"Land\",\"Air\"]");

        Assert.Throws<InvalidDataException>(
            () => MountVehicleRulesLoader.LoadFromJson(json));
    }

    [Fact]
    public void IntegerVehicleKind_IsRejected()
    {
        string json = ValidJson.Replace(
            "[\"Land\",\"Water\"]",
            "[1,\"Water\"]");

        Assert.Throws<InvalidDataException>(
            () => MountVehicleRulesLoader.LoadFromJson(json));
    }

    [Fact]
    public void MissingRowboatOverlandWeight_IsRejected()
    {
        string json = ValidJson.Replace(
            "\"rowboatOverlandWeight\":{\"pounds\":100},",
            string.Empty);

        Assert.Throws<InvalidDataException>(
            () => MountVehicleRulesLoader.LoadFromJson(json));
    }

    [Fact]
    public void MissingSources_IsRejected()
    {
        string json = ValidJson.Replace(
            ",\"sources\":[{\"documentId\":\"dnd5e2014.source.phb-first-printing\",\"page\":155,\"section\":\"Chapter 5: Equipment — Mounts and Vehicles\"}]",
            string.Empty);

        Assert.Throws<InvalidDataException>(
            () => MountVehicleRulesLoader.LoadFromJson(json));
    }

    [Fact]
    public void DuplicateVehicleProficiencyKinds_AreRejectedAsDataError()
    {
        string json = ValidJson.Replace(
            "[\"Land\",\"Water\"]",
            "[\"Land\",\"Land\"]");

        Assert.Throws<InvalidDataException>(
            () => MountVehicleRulesLoader.LoadFromJson(json));
    }
}
