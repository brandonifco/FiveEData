using FiveEData.Rules.Common;

namespace FiveEData.Rules.Equipment.Vehicles;

internal static class VehicleDefinitionValidator
{
    public static IReadOnlyList<string> Validate(VehicleDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(definition.Id.Value))
        {
            errors.Add("Vehicle ID must not be empty.");
        }

        if (string.IsNullOrWhiteSpace(definition.Name))
        {
            errors.Add("Vehicle name must not be empty.");
        }

        if (definition.Cost.CopperPieces <= 0)
        {
            errors.Add("Vehicle cost must be greater than zero.");
        }

        switch (definition.Kind)
        {
            case VehicleKind.Land:
                ValidateLandVehicle(definition, errors);
                break;

            case VehicleKind.Water:
                ValidateWaterVehicle(definition, errors);
                break;

            default:
                errors.Add("Vehicle kind is not recognized.");
                break;
        }

        var ruleIds = new HashSet<RuleId>();

        foreach (RuleId ruleId in definition.SpecialRuleIds)
        {
            if (string.IsNullOrWhiteSpace(ruleId.Value))
            {
                errors.Add("Vehicle special rule ID must not be empty.");
                continue;
            }

            if (!ruleIds.Add(ruleId))
            {
                errors.Add(
                    $"Vehicle special rule ID '{ruleId}' is duplicated.");
            }
        }

        if (definition.Sources.Count == 0)
        {
            errors.Add("Vehicle must have at least one source reference.");
        }

        return errors;
    }

    public static void EnsureValid(VehicleDefinition definition)
    {
        IReadOnlyList<string> errors = Validate(definition);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Vehicle definition '{definition.Id}' is invalid:{Environment.NewLine}" +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }

    private static void ValidateLandVehicle(
        VehicleDefinition definition,
        ICollection<string> errors)
    {
        if (definition.ListedWeight is null)
        {
            errors.Add(
                "Land vehicle must have the listed weight from the source table.");
        }
        else if (definition.ListedWeight.Value.Pounds <= 0)
        {
            errors.Add("Land vehicle listed weight must be greater than zero.");
        }

        if (definition.ListedSpeed is not null)
        {
            errors.Add(
                "Land vehicle must not have a waterborne listed speed.");
        }
    }

    private static void ValidateWaterVehicle(
        VehicleDefinition definition,
        ICollection<string> errors)
    {
        if (definition.ListedWeight is not null)
        {
            errors.Add(
                "Water vehicle must not have a drawn-vehicle listed weight.");
        }

        if (definition.ListedSpeed is null)
        {
            errors.Add(
                "Water vehicle must have the listed speed from the source table.");
        }
        else if (definition.ListedSpeed.Value.MilesPerHour <= 0)
        {
            errors.Add("Water vehicle listed speed must be greater than zero.");
        }
    }
}
