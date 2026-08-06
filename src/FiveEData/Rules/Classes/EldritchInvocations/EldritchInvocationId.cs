namespace FiveEData.Rules.Classes.EldritchInvocations;

public readonly record struct EldritchInvocationId
{
    public EldritchInvocationId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
