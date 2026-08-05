namespace FiveEData.Rules.Classes;

public readonly record struct ClassId
{
    public ClassId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
