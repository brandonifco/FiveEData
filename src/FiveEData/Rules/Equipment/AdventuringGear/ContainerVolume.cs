namespace FiveEData.Rules.Equipment.AdventuringGear;

public readonly record struct ContainerVolume
{
    public ContainerVolume(decimal amount, ContainerVolumeUnit unit)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                amount,
                "Container volume must be greater than zero.");
        }

        if (!Enum.IsDefined(unit))
        {
            throw new ArgumentOutOfRangeException(
                nameof(unit),
                unit,
                "Container volume unit must be defined.");
        }

        Amount = amount;
        Unit = unit;
    }

    public decimal Amount { get; }
    public ContainerVolumeUnit Unit { get; }
}
