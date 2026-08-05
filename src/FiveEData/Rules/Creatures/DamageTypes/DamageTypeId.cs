namespace FiveEData.Rules.Creatures.DamageTypes;

public readonly record struct DamageTypeId
{
    public DamageTypeId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
