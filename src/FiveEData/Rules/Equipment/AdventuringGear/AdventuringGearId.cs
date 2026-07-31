namespace FiveEData.Rules.Equipment.AdventuringGear;

public readonly record struct AdventuringGearId
{
    public AdventuringGearId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
