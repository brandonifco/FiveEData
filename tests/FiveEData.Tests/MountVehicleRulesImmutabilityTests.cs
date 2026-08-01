using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment.MountsAndVehicles;
using FiveEData.Rules.Equipment.Vehicles;

namespace FiveEData.Tests;

public sealed class MountVehicleRulesImmutabilityTests
{
    [Fact]
    public void Rules_DefensivelySnapshotCollectionInputs()
    {
        var kinds = new List<VehicleKind>
        {
            VehicleKind.Land,
            VehicleKind.Water
        };
        var sources = new List<SourceReference>
        {
            new(
                new SourceDocumentId(
                    "dnd5e2014.source.phb-first-printing"),
                page: 155)
        };

        var rules = new MountVehicleRules(
            new RuleId(
                "dnd5e2014.mount-vehicle-rule.drawn-vehicle-pulling-capacity"),
            5,
            drawnVehicleCapacityIncludesVehicleWeight: true,
            multipleAnimalsCombineCarryingCapacity: true,
            new RuleId(
                "dnd5e2014.mount-vehicle-rule.other-mount-availability"),
            otherMountsAreRare: true,
            otherMountsNormallyAvailableForPurchase: false,
            new RuleId("dnd5e2014.mount-vehicle-rule.barding"),
            bardingAvailableForAnyArmorType: true,
            bardingCostMultiplier: 4,
            bardingWeightMultiplier: 2,
            new RuleId(
                "dnd5e2014.mount-vehicle-rule.military-saddle"),
            militarySaddleGrantsAdvantageOnChecksToRemainMounted: true,
            new RuleId(
                "dnd5e2014.mount-vehicle-rule.exotic-saddle"),
            exoticSaddleRequiredForAquaticOrFlyingMounts: true,
            new RuleId(
                "dnd5e2014.mount-vehicle-rule.vehicle-proficiency"),
            kinds,
            vehicleProficiencyAddsProficiencyBonusToDifficultControlChecks:
                true,
            new RuleId(
                "dnd5e2014.mount-vehicle-rule.rowed-vessels"),
            new VehicleSpeed(3),
            downstreamCurrentAddsToVehicleSpeed: true,
            rowedVesselsCanBeRowedAgainstSignificantCurrent: false,
            rowedVesselsCanBePulledUpstreamByDraftAnimals: true,
            new VehicleId("dnd5e2014.vehicle.rowboat"),
            new Weight(100),
            sources);

        kinds.Clear();
        sources.Clear();

        Assert.Equal(
            new[] { VehicleKind.Land, VehicleKind.Water },
            rules.VehicleProficiencyKinds);
        Assert.Single(rules.Sources);
        Assert.Equal(7, rules.ReferencedRuleIds.Count);
    }
}
