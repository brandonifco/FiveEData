namespace FiveEData.Rules.Equipment;

public readonly record struct EquipmentChangeTiming
{
    public EquipmentChangeTiming(
        EquipmentChangeDuration don,
        EquipmentChangeDuration doff)
    {
        EnsureValid(don, nameof(don));
        EnsureValid(doff, nameof(doff));

        Don = don;
        Doff = doff;
    }

    public EquipmentChangeDuration Don { get; }
    public EquipmentChangeDuration Doff { get; }

    private static void EnsureValid(
        EquipmentChangeDuration duration,
        string parameterName)
    {
        if (duration.Amount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                duration.Amount,
                "Equipment-change timing durations must be greater than zero.");
        }

        if (!Enum.IsDefined(duration.Unit))
        {
            throw new ArgumentOutOfRangeException(
                parameterName,
                duration.Unit,
                "Equipment-change timing duration unit is not defined.");
        }
    }
}
