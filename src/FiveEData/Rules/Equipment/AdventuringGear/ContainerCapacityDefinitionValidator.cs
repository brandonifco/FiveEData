namespace FiveEData.Rules.Equipment.AdventuringGear;

internal static class ContainerCapacityDefinitionValidator
{
    public static IReadOnlyList<string> Validate(
        ContainerCapacityDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);

        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(definition.AdventuringGearId.Value))
        {
            errors.Add("Container-capacity adventuring gear ID must not be empty.");
        }

        ValidateVolume(
            definition.SolidVolume,
            isSolid: true,
            nameof(definition.SolidVolume),
            errors);
        ValidateVolume(
            definition.LiquidVolume,
            isSolid: false,
            nameof(definition.LiquidVolume),
            errors);

        if (definition.GearWeightCapacity is { Pounds: <= 0 })
        {
            errors.Add(
                "Container gear-weight capacity must be greater than zero when specified.");
        }

        if (definition.SolidVolume is null &&
            definition.LiquidVolume is null &&
            definition.GearWeightCapacity is null)
        {
            errors.Add(
                "Container capacity must specify at least one capacity measure.");
        }

        if (definition.Sources.Count == 0)
        {
            errors.Add(
                "Container capacity must have at least one source reference.");
        }

        return errors;
    }

    public static void EnsureValid(ContainerCapacityDefinition definition)
    {
        IReadOnlyList<string> errors = Validate(definition);

        if (errors.Count == 0)
        {
            return;
        }

        throw new InvalidOperationException(
            $"Container capacity for adventuring gear '{definition.AdventuringGearId}' is invalid:" +
            Environment.NewLine +
            string.Join(
                Environment.NewLine,
                errors.Select(error => $"- {error}")));
    }

    private static void ValidateVolume(
        ContainerVolume? volume,
        bool isSolid,
        string propertyName,
        ICollection<string> errors)
    {
        if (volume is null)
        {
            return;
        }

        if (volume.Value.Amount <= 0 || !Enum.IsDefined(volume.Value.Unit))
        {
            errors.Add(
                $"Container volume '{propertyName}' must have a positive amount and defined unit.");
            return;
        }

        if (isSolid && volume.Value.Unit != ContainerVolumeUnit.CubicFoot)
        {
            errors.Add(
                "Solid container volume must use cubic feet.");
        }

        if (!isSolid && volume.Value.Unit == ContainerVolumeUnit.CubicFoot)
        {
            errors.Add(
                "Liquid container volume must use a liquid-volume unit.");
        }
    }
}
