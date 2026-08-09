namespace FiveEData.Rules.Combat.Cover;

public readonly record struct CoverId
{
    public CoverId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
