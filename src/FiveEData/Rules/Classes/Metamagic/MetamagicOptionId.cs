namespace FiveEData.Rules.Classes.Metamagic;

public readonly record struct MetamagicOptionId
{
    public MetamagicOptionId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
