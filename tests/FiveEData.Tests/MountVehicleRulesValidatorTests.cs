using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment.MountsAndVehicles;
using FiveEData.Rules.Equipment.Vehicles;

namespace FiveEData.Tests;

public sealed class MountVehicleRulesValidatorTests
{
    [Fact]
    public void DefaultRuleId_IsRejected()
    {
        MountVehicleRules rules = Create(
            drawnVehiclePullingRuleId: default(RuleId));

        Assert.Contains(
            MountVehicleRulesValidator.Validate(rules),
            error => error.Contains(
                "must not be empty",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void DuplicateRuleIds_AreRejected()
    {
        RuleId duplicate =
            new("dnd5e2014.mount-vehicle-rule.duplicate");

        MountVehicleRules rules = Create(
            drawnVehiclePullingRuleId: duplicate,
            bardingRuleId: duplicate);

        Assert.Contains(
            MountVehicleRulesValidator.Validate(rules),
            error => error.Contains(
                "duplicated",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void DrawnVehicleFacts_AreValidated()
    {
        MountVehicleRules rules = Create(
            drawnVehicleCarryingCapacityMultiplier: 0,
            drawnVehicleCapacityIncludesVehicleWeight: false,
            multipleAnimalsCombineCarryingCapacity: false);

        IReadOnlyList<string> errors =
            MountVehicleRulesValidator.Validate(rules);

        Assert.Contains(
            errors,
            error => error.Contains(
                "multiplier",
                StringComparison.OrdinalIgnoreCase));
        Assert.Contains(
            errors,
            error => error.Contains(
                "vehicle weight",
                StringComparison.OrdinalIgnoreCase));
        Assert.Contains(
            errors,
            error => error.Contains(
                "Multiple animals",
                StringComparison.Ordinal));
    }

    [Fact]
    public void PositiveButNoncanonicalNumericFacts_AreRejected()
    {
        Assert.Contains(
            MountVehicleRulesValidator.Validate(
                Create(drawnVehicleCarryingCapacityMultiplier: 6)),
            error => error.Contains(
                "exactly 5",
                StringComparison.Ordinal));

        Assert.Contains(
            MountVehicleRulesValidator.Validate(
                Create(bardingCostMultiplier: 5)),
            error => error.Contains(
                "exactly 4",
                StringComparison.Ordinal));

        Assert.Contains(
            MountVehicleRulesValidator.Validate(
                Create(bardingWeightMultiplier: 3)),
            error => error.Contains(
                "exactly 2",
                StringComparison.Ordinal));

        Assert.Contains(
            MountVehicleRulesValidator.Validate(
                Create(typicalCurrentSpeed: new VehicleSpeed(4))),
            error => error.Contains(
                "exactly 3",
                StringComparison.Ordinal));

        Assert.Contains(
            MountVehicleRulesValidator.Validate(
                Create(rowboatOverlandWeight: new Weight(101))),
            error => error.Contains(
                "exactly 100",
                StringComparison.Ordinal));
    }

    [Fact]
    public void OtherMountAvailabilityFacts_AreValidated()
    {
        MountVehicleRules rules = Create(
            otherMountsAreRare: false,
            otherMountsNormallyAvailableForPurchase: true);

        IReadOnlyList<string> errors =
            MountVehicleRulesValidator.Validate(rules);

        Assert.Contains(
            errors,
            error => error.Contains(
                "rare",
                StringComparison.OrdinalIgnoreCase));
        Assert.Contains(
            errors,
            error => error.Contains(
                "normally available",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void BardingFacts_AreValidated()
    {
        MountVehicleRules rules = Create(
            bardingAvailableForAnyArmorType: false,
            bardingCostMultiplier: 0,
            bardingWeightMultiplier: 0);

        IReadOnlyList<string> errors =
            MountVehicleRulesValidator.Validate(rules);

        Assert.Equal(
            3,
            errors.Count(error => error.Contains(
                "Barding",
                StringComparison.OrdinalIgnoreCase)));
    }

    [Fact]
    public void SaddleFacts_AreValidated()
    {
        MountVehicleRules rules = Create(
            militarySaddleGrantsAdvantageOnChecksToRemainMounted: false,
            exoticSaddleRequiredForAquaticOrFlyingMounts: false);

        IReadOnlyList<string> errors =
            MountVehicleRulesValidator.Validate(rules);

        Assert.Contains(
            errors,
            error => error.Contains(
                "Military saddle",
                StringComparison.OrdinalIgnoreCase));
        Assert.Contains(
            errors,
            error => error.Contains(
                "Exotic saddle",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void VehicleProficiencyFacts_AreValidated()
    {
        MountVehicleRules rules = Create(
            vehicleProficiencyKinds:
            [
                VehicleKind.Land,
                VehicleKind.Land
            ],
            vehicleProficiencyAddsProficiencyBonusToDifficultControlChecks:
                false);

        IReadOnlyList<string> errors =
            MountVehicleRulesValidator.Validate(rules);

        Assert.Contains(
            errors,
            error => error.Contains(
                "exactly Land and Water",
                StringComparison.Ordinal));
        Assert.Contains(
            errors,
            error => error.Contains(
                "proficiency bonus",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void RowedVesselFacts_AreValidated()
    {
        MountVehicleRules rules = Create(
            typicalCurrentSpeed: new VehicleSpeed(0),
            downstreamCurrentAddsToVehicleSpeed: false,
            rowedVesselsCanBeRowedAgainstSignificantCurrent: true,
            rowedVesselsCanBePulledUpstreamByDraftAnimals: false);

        IReadOnlyList<string> errors =
            MountVehicleRulesValidator.Validate(rules);

        Assert.Contains(
            errors,
            error => error.Contains(
                "current speed",
                StringComparison.OrdinalIgnoreCase));
        Assert.Contains(
            errors,
            error => error.Contains(
                "Downstream current",
                StringComparison.Ordinal));
        Assert.Contains(
            errors,
            error => error.Contains(
                "significant current",
                StringComparison.OrdinalIgnoreCase));
        Assert.Contains(
            errors,
            error => error.Contains(
                "draft animals",
                StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void RowboatAndSourceFacts_AreValidated()
    {
        MountVehicleRules rules = Create(
            rowboatVehicleId: default(VehicleId),
            rowboatOverlandWeight: new Weight(0),
            sources: []);

        IReadOnlyList<string> errors =
            MountVehicleRulesValidator.Validate(rules);

        Assert.Contains(
            errors,
            error => error.Contains(
                "Rowboat vehicle ID",
                StringComparison.Ordinal));
        Assert.Contains(
            errors,
            error => error.Contains(
                "overland weight",
                StringComparison.OrdinalIgnoreCase));
        Assert.Contains(
            errors,
            error => error.Contains(
                "source reference",
                StringComparison.OrdinalIgnoreCase));
    }

    private static MountVehicleRules Create(
        RuleId? drawnVehiclePullingRuleId = null,
        int drawnVehicleCarryingCapacityMultiplier = 5,
        bool drawnVehicleCapacityIncludesVehicleWeight = true,
        bool multipleAnimalsCombineCarryingCapacity = true,
        RuleId? otherMountAvailabilityRuleId = null,
        bool otherMountsAreRare = true,
        bool otherMountsNormallyAvailableForPurchase = false,
        RuleId? bardingRuleId = null,
        bool bardingAvailableForAnyArmorType = true,
        int bardingCostMultiplier = 4,
        int bardingWeightMultiplier = 2,
        RuleId? militarySaddleRuleId = null,
        bool militarySaddleGrantsAdvantageOnChecksToRemainMounted = true,
        RuleId? exoticSaddleRuleId = null,
        bool exoticSaddleRequiredForAquaticOrFlyingMounts = true,
        RuleId? vehicleProficiencyRuleId = null,
        IEnumerable<VehicleKind>? vehicleProficiencyKinds = null,
        bool vehicleProficiencyAddsProficiencyBonusToDifficultControlChecks =
            true,
        RuleId? rowedVesselsRuleId = null,
        VehicleSpeed? typicalCurrentSpeed = null,
        bool downstreamCurrentAddsToVehicleSpeed = true,
        bool rowedVesselsCanBeRowedAgainstSignificantCurrent = false,
        bool rowedVesselsCanBePulledUpstreamByDraftAnimals = true,
        VehicleId? rowboatVehicleId = null,
        Weight? rowboatOverlandWeight = null,
        IEnumerable<SourceReference>? sources = null)
    {
        return new MountVehicleRules(
            drawnVehiclePullingRuleId ??
                new RuleId(
                    "dnd5e2014.mount-vehicle-rule.drawn-vehicle-pulling-capacity"),
            drawnVehicleCarryingCapacityMultiplier,
            drawnVehicleCapacityIncludesVehicleWeight,
            multipleAnimalsCombineCarryingCapacity,
            otherMountAvailabilityRuleId ??
                new RuleId(
                    "dnd5e2014.mount-vehicle-rule.other-mount-availability"),
            otherMountsAreRare,
            otherMountsNormallyAvailableForPurchase,
            bardingRuleId ??
                new RuleId("dnd5e2014.mount-vehicle-rule.barding"),
            bardingAvailableForAnyArmorType,
            bardingCostMultiplier,
            bardingWeightMultiplier,
            militarySaddleRuleId ??
                new RuleId(
                    "dnd5e2014.mount-vehicle-rule.military-saddle"),
            militarySaddleGrantsAdvantageOnChecksToRemainMounted,
            exoticSaddleRuleId ??
                new RuleId(
                    "dnd5e2014.mount-vehicle-rule.exotic-saddle"),
            exoticSaddleRequiredForAquaticOrFlyingMounts,
            vehicleProficiencyRuleId ??
                new RuleId(
                    "dnd5e2014.mount-vehicle-rule.vehicle-proficiency"),
            vehicleProficiencyKinds ??
            [
                VehicleKind.Land,
                VehicleKind.Water
            ],
            vehicleProficiencyAddsProficiencyBonusToDifficultControlChecks,
            rowedVesselsRuleId ??
                new RuleId(
                    "dnd5e2014.mount-vehicle-rule.rowed-vessels"),
            typicalCurrentSpeed ?? new VehicleSpeed(3),
            downstreamCurrentAddsToVehicleSpeed,
            rowedVesselsCanBeRowedAgainstSignificantCurrent,
            rowedVesselsCanBePulledUpstreamByDraftAnimals,
            rowboatVehicleId ??
                new VehicleId("dnd5e2014.vehicle.rowboat"),
            rowboatOverlandWeight ?? new Weight(100),
            sources ??
            [
                new SourceReference(
                    new SourceDocumentId(
                        "dnd5e2014.source.phb-first-printing"),
                    page: 155)
            ]);
    }
}
