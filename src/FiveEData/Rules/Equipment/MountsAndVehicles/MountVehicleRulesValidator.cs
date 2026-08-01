using FiveEData.Rules.Common;
using FiveEData.Rules.Equipment.Vehicles;

namespace FiveEData.Rules.Equipment.MountsAndVehicles;

internal static class MountVehicleRulesValidator
{
    private const int OfficialDrawnVehicleCarryingCapacityMultiplier = 5;
    private const int OfficialBardingCostMultiplier = 4;
    private const int OfficialBardingWeightMultiplier = 2;
    private const decimal OfficialTypicalCurrentSpeedMilesPerHour = 3m;
    private const decimal OfficialRowboatOverlandWeightPounds = 100m;

    public static IReadOnlyList<string> Validate(MountVehicleRules rules)
    {
        ArgumentNullException.ThrowIfNull(rules);

        var errors = new List<string>();
        var ruleIds = new HashSet<RuleId>();

        foreach (RuleId ruleId in rules.ReferencedRuleIds)
        {
            if (string.IsNullOrWhiteSpace(ruleId.Value))
            {
                errors.Add(
                    "Mount and vehicle rule references must not be empty.");
                continue;
            }

            if (!ruleIds.Add(ruleId))
            {
                errors.Add(
                    $"Mount and vehicle rule ID '{ruleId}' is duplicated.");
            }
        }

        if (rules.DrawnVehicleCarryingCapacityMultiplier !=
            OfficialDrawnVehicleCarryingCapacityMultiplier)
        {
            errors.Add(
                $"Drawn-vehicle carrying-capacity multiplier must be exactly {OfficialDrawnVehicleCarryingCapacityMultiplier} for the official ruleset.");
        }

        if (!rules.DrawnVehicleCapacityIncludesVehicleWeight)
        {
            errors.Add(
                "Drawn-vehicle capacity must include the vehicle weight.");
        }

        if (!rules.MultipleAnimalsCombineCarryingCapacity)
        {
            errors.Add(
                "Multiple animals must be able to combine carrying capacity.");
        }

        if (!rules.OtherMountsAreRare)
        {
            errors.Add("Other mounts must be marked as rare.");
        }

        if (rules.OtherMountsNormallyAvailableForPurchase)
        {
            errors.Add(
                "Other mounts must not be marked as normally available for purchase.");
        }

        if (!rules.BardingAvailableForAnyArmorType)
        {
            errors.Add(
                "Barding must be available for any armor type from the Armor table.");
        }

        if (rules.BardingCostMultiplier != OfficialBardingCostMultiplier)
        {
            errors.Add(
                $"Barding cost multiplier must be exactly {OfficialBardingCostMultiplier} for the official ruleset.");
        }

        if (rules.BardingWeightMultiplier != OfficialBardingWeightMultiplier)
        {
            errors.Add(
                $"Barding weight multiplier must be exactly {OfficialBardingWeightMultiplier} for the official ruleset.");
        }

        if (!rules.MilitarySaddleGrantsAdvantageOnChecksToRemainMounted)
        {
            errors.Add(
                "Military saddle must grant advantage on checks to remain mounted.");
        }

        if (!rules.ExoticSaddleRequiredForAquaticOrFlyingMounts)
        {
            errors.Add(
                "Exotic saddle must be required for aquatic or flying mounts.");
        }

        ValidateVehicleProficiencyKinds(rules, errors);

        if (!rules.VehicleProficiencyAddsProficiencyBonusToDifficultControlChecks)
        {
            errors.Add(
                "Vehicle proficiency must add proficiency bonus to difficult control checks.");
        }

        if (rules.TypicalCurrentSpeed.MilesPerHour !=
            OfficialTypicalCurrentSpeedMilesPerHour)
        {
            errors.Add(
                $"Typical current speed must be exactly {OfficialTypicalCurrentSpeedMilesPerHour} mph for the official ruleset.");
        }

        if (!rules.DownstreamCurrentAddsToVehicleSpeed)
        {
            errors.Add(
                "Downstream current must add to rowed-vessel speed.");
        }

        if (rules.RowedVesselsCanBeRowedAgainstSignificantCurrent)
        {
            errors.Add(
                "Rowed vessels must not be rowable against significant current.");
        }

        if (!rules.RowedVesselsCanBePulledUpstreamByDraftAnimals)
        {
            errors.Add(
                "Rowed vessels must be pullable upstream by draft animals.");
        }

        if (string.IsNullOrWhiteSpace(rules.RowboatVehicleId.Value))
        {
            errors.Add("Rowboat vehicle ID must not be empty.");
        }

        if (rules.RowboatOverlandWeight.Pounds !=
            OfficialRowboatOverlandWeightPounds)
        {
            errors.Add(
                $"Rowboat overland weight must be exactly {OfficialRowboatOverlandWeightPounds} pounds for the official ruleset.");
        }

        if (rules.Sources.Count == 0)
        {
            errors.Add(
                "Mount and vehicle rules must have at least one source reference.");
        }

        return errors;
    }

    public static void EnsureValid(MountVehicleRules rules)
    {
        IReadOnlyList<string> errors = Validate(rules);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Mount and vehicle rules are invalid:{Environment.NewLine}" +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }

    private static void ValidateVehicleProficiencyKinds(
        MountVehicleRules rules,
        ICollection<string> errors)
    {
        var kinds = new HashSet<VehicleKind>();

        foreach (VehicleKind kind in rules.VehicleProficiencyKinds)
        {
            if (!Enum.IsDefined(kind))
            {
                errors.Add(
                    $"Vehicle proficiency kind '{kind}' is not recognized.");
                continue;
            }

            if (!kinds.Add(kind))
            {
                errors.Add(
                    $"Vehicle proficiency kind '{kind}' is duplicated.");
            }
        }

        if (kinds.Count != 2 ||
            !kinds.Contains(VehicleKind.Land) ||
            !kinds.Contains(VehicleKind.Water))
        {
            errors.Add(
                "Vehicle proficiency kinds must contain exactly Land and Water.");
        }
    }
}
