using FiveEData.Rules.Common;
using FiveEData.Rules.Common.Provenance;
using FiveEData.Rules.Common.Provenance.Serialization;
using FiveEData.Rules.Common.Serialization;
using FiveEData.Rules.Equipment.Vehicles;

namespace FiveEData.Rules.Equipment.MountsAndVehicles.Serialization;

internal static class MountVehicleRulesLoader
{
    public static MountVehicleRules LoadFromFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static MountVehicleRules LoadFromJson(string json)
    {
        MountVehicleRulesData data =
            StrictJson.DeserializeObject<MountVehicleRulesData>(
                json,
                "Mount and vehicle rules");

        try
        {
            MountVehicleRules rules = Map(data);
            MountVehicleRulesValidator.EnsureValid(rules);
            return rules;
        }
        catch (Exception exception)
            when (exception is ArgumentException or InvalidOperationException)
        {
            throw new InvalidDataException(
                "Invalid mount and vehicle rules definition.",
                exception);
        }
    }

    private static MountVehicleRules Map(MountVehicleRulesData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        VehicleKind[] vehicleProficiencyKinds =
            data.VehicleProficiencyKinds
            ?? throw new ArgumentException(
                "Vehicle proficiency kinds are required.",
                nameof(data));

        WeightData rowboatOverlandWeight =
            data.RowboatOverlandWeight
            ?? throw new ArgumentException(
                "Rowboat overland weight is required.",
                nameof(data));

        SourceReferenceData[] sourceData =
            data.Sources
            ?? throw new ArgumentException(
                "Mount and vehicle rule sources are required.",
                nameof(data));

        SourceReference[] sources = sourceData
            .Select(SourceReferenceDataMapper.Map)
            .ToArray();

        return new MountVehicleRules(
            CreateRuleId(
                data.DrawnVehiclePullingRuleId,
                "drawn-vehicle pulling"),
            data.DrawnVehicleCarryingCapacityMultiplier,
            data.DrawnVehicleCapacityIncludesVehicleWeight,
            data.MultipleAnimalsCombineCarryingCapacity,
            CreateRuleId(
                data.OtherMountAvailabilityRuleId,
                "other-mount availability"),
            data.OtherMountsAreRare,
            data.OtherMountsNormallyAvailableForPurchase,
            CreateRuleId(data.BardingRuleId, "barding"),
            data.BardingAvailableForAnyArmorType,
            data.BardingCostMultiplier,
            data.BardingWeightMultiplier,
            CreateRuleId(
                data.MilitarySaddleRuleId,
                "military-saddle"),
            data.MilitarySaddleGrantsAdvantageOnChecksToRemainMounted,
            CreateRuleId(
                data.ExoticSaddleRuleId,
                "exotic-saddle"),
            data.ExoticSaddleRequiredForAquaticOrFlyingMounts,
            CreateRuleId(
                data.VehicleProficiencyRuleId,
                "vehicle-proficiency"),
            vehicleProficiencyKinds,
            data.VehicleProficiencyAddsProficiencyBonusToDifficultControlChecks,
            CreateRuleId(
                data.RowedVesselsRuleId,
                "rowed-vessels"),
            new VehicleSpeed(data.TypicalCurrentSpeedMilesPerHour),
            data.DownstreamCurrentAddsToVehicleSpeed,
            data.RowedVesselsCanBeRowedAgainstSignificantCurrent,
            data.RowedVesselsCanBePulledUpstreamByDraftAnimals,
            new VehicleId(
                data.RowboatVehicleId
                ?? throw new ArgumentException(
                    "Rowboat vehicle ID is required.",
                    nameof(data))),
            new Weight(rowboatOverlandWeight.Pounds),
            sources);
    }

    private static RuleId CreateRuleId(
        string? value,
        string description)
    {
        return new RuleId(
            value
            ?? throw new ArgumentException(
                $"Mount and vehicle {description} rule ID is required."));
    }
}
