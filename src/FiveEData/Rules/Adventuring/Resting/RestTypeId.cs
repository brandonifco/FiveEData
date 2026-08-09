namespace FiveEData.Rules.Adventuring.Resting;

public readonly record struct RestTypeId
{
    public RestTypeId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
