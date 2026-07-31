namespace FiveEData.Rules.Common;

public readonly record struct RuleId
{
    public RuleId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
