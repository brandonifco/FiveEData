namespace FiveEData.Rules.Equipment.Vehicles;

public readonly record struct VehicleSpeed
{
    public VehicleSpeed(decimal milesPerHour)
    {
        if (milesPerHour < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(milesPerHour),
                milesPerHour,
                "Vehicle speed cannot be negative.");
        }

        MilesPerHour = milesPerHour;
    }

    public decimal MilesPerHour { get; }
}
