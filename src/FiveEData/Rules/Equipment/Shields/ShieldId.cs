namespace FiveEData.Rules.Equipment.Shields;

public readonly record struct ShieldId
{
    public ShieldId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
