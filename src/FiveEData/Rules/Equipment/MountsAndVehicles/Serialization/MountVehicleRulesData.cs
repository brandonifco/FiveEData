using System.Text.Json.Serialization;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Equipment.Vehicles;

namespace FiveEData.Rules.Equipment.MountsAndVehicles.Serialization;

internal sealed class MountVehicleRulesData
{
    [JsonRequired]
    public string? DrawnVehiclePullingRuleId { get; init; }

    [JsonRequired]
    public int DrawnVehicleCarryingCapacityMultiplier { get; init; }

    [JsonRequired]
    public bool DrawnVehicleCapacityIncludesVehicleWeight { get; init; }

    [JsonRequired]
    public bool MultipleAnimalsCombineCarryingCapacity { get; init; }

    [JsonRequired]
    public string? OtherMountAvailabilityRuleId { get; init; }

    [JsonRequired]
    public bool OtherMountsAreRare { get; init; }

    [JsonRequired]
    public bool OtherMountsNormallyAvailableForPurchase { get; init; }

    [JsonRequired]
    public string? BardingRuleId { get; init; }

    [JsonRequired]
    public bool BardingAvailableForAnyArmorType { get; init; }

    [JsonRequired]
    public int BardingCostMultiplier { get; init; }

    [JsonRequired]
    public int BardingWeightMultiplier { get; init; }

    [JsonRequired]
    public string? MilitarySaddleRuleId { get; init; }

    [JsonRequired]
    public bool MilitarySaddleGrantsAdvantageOnChecksToRemainMounted
    {
        get;
        init;
    }

    [JsonRequired]
    public string? ExoticSaddleRuleId { get; init; }

    [JsonRequired]
    public bool ExoticSaddleRequiredForAquaticOrFlyingMounts { get; init; }

    [JsonRequired]
    public string? VehicleProficiencyRuleId { get; init; }

    [JsonRequired]
    public VehicleKind[]? VehicleProficiencyKinds { get; init; }

    [JsonRequired]
    public bool VehicleProficiencyAddsProficiencyBonusToDifficultControlChecks
    {
        get;
        init;
    }

    [JsonRequired]
    public string? RowedVesselsRuleId { get; init; }

    [JsonRequired]
    public decimal TypicalCurrentSpeedMilesPerHour { get; init; }

    [JsonRequired]
    public bool DownstreamCurrentAddsToVehicleSpeed { get; init; }

    [JsonRequired]
    public bool RowedVesselsCanBeRowedAgainstSignificantCurrent { get; init; }

    [JsonRequired]
    public bool RowedVesselsCanBePulledUpstreamByDraftAnimals { get; init; }

    [JsonRequired]
    public string? RowboatVehicleId { get; init; }

    [JsonRequired]
    public WeightData? RowboatOverlandWeight { get; init; }

    [JsonRequired]
    public SourceReferenceData[]? Sources { get; init; }
}
