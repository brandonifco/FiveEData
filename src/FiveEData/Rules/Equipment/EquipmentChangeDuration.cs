namespace FiveEData.Rules.Equipment;

public readonly record struct EquipmentChangeDuration
{
    public EquipmentChangeDuration(
        int amount,
        EquipmentChangeTimeUnit unit)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                amount,
                "Equipment-change duration must be greater than zero.");
        }

        if (!Enum.IsDefined(unit))
        {
            throw new ArgumentOutOfRangeException(
                nameof(unit),
                unit,
                "Equipment-change time unit is not defined.");
        }

        Amount = amount;
        Unit = unit;
    }

    public int Amount { get; }
    public EquipmentChangeTimeUnit Unit { get; }
}
