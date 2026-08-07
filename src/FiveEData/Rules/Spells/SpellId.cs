namespace FiveEData.Rules.Spells;

public readonly record struct SpellId
{
    public SpellId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
