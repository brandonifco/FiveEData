namespace FiveEData.Rules.Creatures.Senses;

public readonly record struct SenseId
{
    public SenseId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
