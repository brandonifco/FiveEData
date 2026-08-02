namespace FiveEData.Rules.Expenses.Services;

public readonly record struct MundaneServiceId
{
    public MundaneServiceId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
