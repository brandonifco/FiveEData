using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Equipment.Vehicles;

namespace FiveEData.Rules.Equipment.MountsAndVehicles;

public sealed class MountVehicleRules
{
    private readonly IReadOnlyList<RuleId> _referencedRuleIds;
    private readonly IReadOnlyList<VehicleKind> _vehicleProficiencyKinds;

    internal MountVehicleRules(
        RuleId drawnVehiclePullingRuleId,
        int drawnVehicleCarryingCapacityMultiplier,
        bool drawnVehicleCapacityIncludesVehicleWeight,
        bool multipleAnimalsCombineCarryingCapacity,
        RuleId otherMountAvailabilityRuleId,
        bool otherMountsAreRare,
        bool otherMountsNormallyAvailableForPurchase,
        RuleId bardingRuleId,
        bool bardingAvailableForAnyArmorType,
        int bardingCostMultiplier,
        int bardingWeightMultiplier,
        RuleId militarySaddleRuleId,
        bool militarySaddleGrantsAdvantageOnChecksToRemainMounted,
        RuleId exoticSaddleRuleId,
        bool exoticSaddleRequiredForAquaticOrFlyingMounts,
        RuleId vehicleProficiencyRuleId,
        IEnumerable<VehicleKind> vehicleProficiencyKinds,
        bool vehicleProficiencyAddsProficiencyBonusToDifficultControlChecks,
        RuleId rowedVesselsRuleId,
        VehicleSpeed typicalCurrentSpeed,
        bool downstreamCurrentAddsToVehicleSpeed,
        bool rowedVesselsCanBeRowedAgainstSignificantCurrent,
        bool rowedVesselsCanBePulledUpstreamByDraftAnimals,
        VehicleId rowboatVehicleId,
        Weight rowboatOverlandWeight,
        IEnumerable<SourceReference> sources)
    {
        ArgumentNullException.ThrowIfNull(vehicleProficiencyKinds);
        ArgumentNullException.ThrowIfNull(sources);

        DrawnVehiclePullingRuleId = drawnVehiclePullingRuleId;
        DrawnVehicleCarryingCapacityMultiplier =
            drawnVehicleCarryingCapacityMultiplier;
        DrawnVehicleCapacityIncludesVehicleWeight =
            drawnVehicleCapacityIncludesVehicleWeight;
        MultipleAnimalsCombineCarryingCapacity =
            multipleAnimalsCombineCarryingCapacity;
        OtherMountAvailabilityRuleId = otherMountAvailabilityRuleId;
        OtherMountsAreRare = otherMountsAreRare;
        OtherMountsNormallyAvailableForPurchase =
            otherMountsNormallyAvailableForPurchase;
        BardingRuleId = bardingRuleId;
        BardingAvailableForAnyArmorType = bardingAvailableForAnyArmorType;
        BardingCostMultiplier = bardingCostMultiplier;
        BardingWeightMultiplier = bardingWeightMultiplier;
        MilitarySaddleRuleId = militarySaddleRuleId;
        MilitarySaddleGrantsAdvantageOnChecksToRemainMounted =
            militarySaddleGrantsAdvantageOnChecksToRemainMounted;
        ExoticSaddleRuleId = exoticSaddleRuleId;
        ExoticSaddleRequiredForAquaticOrFlyingMounts =
            exoticSaddleRequiredForAquaticOrFlyingMounts;
        VehicleProficiencyRuleId = vehicleProficiencyRuleId;
        _vehicleProficiencyKinds = Array.AsReadOnly(
            vehicleProficiencyKinds.ToArray());
        VehicleProficiencyAddsProficiencyBonusToDifficultControlChecks =
            vehicleProficiencyAddsProficiencyBonusToDifficultControlChecks;
        RowedVesselsRuleId = rowedVesselsRuleId;
        TypicalCurrentSpeed = typicalCurrentSpeed;
        DownstreamCurrentAddsToVehicleSpeed =
            downstreamCurrentAddsToVehicleSpeed;
        RowedVesselsCanBeRowedAgainstSignificantCurrent =
            rowedVesselsCanBeRowedAgainstSignificantCurrent;
        RowedVesselsCanBePulledUpstreamByDraftAnimals =
            rowedVesselsCanBePulledUpstreamByDraftAnimals;
        RowboatVehicleId = rowboatVehicleId;
        RowboatOverlandWeight = rowboatOverlandWeight;
        Sources = Array.AsReadOnly(sources.ToArray());

        _referencedRuleIds = Array.AsReadOnly(
            new[]
            {
                DrawnVehiclePullingRuleId,
                OtherMountAvailabilityRuleId,
                BardingRuleId,
                MilitarySaddleRuleId,
                ExoticSaddleRuleId,
                VehicleProficiencyRuleId,
                RowedVesselsRuleId
            });
    }

    public RuleId DrawnVehiclePullingRuleId { get; }
    public int DrawnVehicleCarryingCapacityMultiplier { get; }
    public bool DrawnVehicleCapacityIncludesVehicleWeight { get; }
    public bool MultipleAnimalsCombineCarryingCapacity { get; }
    public RuleId OtherMountAvailabilityRuleId { get; }
    public bool OtherMountsAreRare { get; }
    public bool OtherMountsNormallyAvailableForPurchase { get; }
    public RuleId BardingRuleId { get; }
    public bool BardingAvailableForAnyArmorType { get; }
    public int BardingCostMultiplier { get; }
    public int BardingWeightMultiplier { get; }
    public RuleId MilitarySaddleRuleId { get; }
    public bool MilitarySaddleGrantsAdvantageOnChecksToRemainMounted { get; }
    public RuleId ExoticSaddleRuleId { get; }
    public bool ExoticSaddleRequiredForAquaticOrFlyingMounts { get; }
    public RuleId VehicleProficiencyRuleId { get; }
    public IReadOnlyList<VehicleKind> VehicleProficiencyKinds =>
        _vehicleProficiencyKinds;
    public bool VehicleProficiencyAddsProficiencyBonusToDifficultControlChecks
    {
        get;
    }
    public RuleId RowedVesselsRuleId { get; }
    public VehicleSpeed TypicalCurrentSpeed { get; }
    public bool DownstreamCurrentAddsToVehicleSpeed { get; }
    public bool RowedVesselsCanBeRowedAgainstSignificantCurrent { get; }
    public bool RowedVesselsCanBePulledUpstreamByDraftAnimals { get; }
    public VehicleId RowboatVehicleId { get; }
    public Weight RowboatOverlandWeight { get; }
    public IReadOnlyList<SourceReference> Sources { get; }
    public IReadOnlyList<RuleId> ReferencedRuleIds => _referencedRuleIds;
}
