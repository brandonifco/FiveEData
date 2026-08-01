namespace FiveEData.Rules.Equipment.Vehicles;

public readonly record struct VehicleId
{
    public VehicleId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
