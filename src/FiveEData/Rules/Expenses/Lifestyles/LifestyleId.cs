namespace FiveEData.Rules.Expenses.Lifestyles;

public readonly record struct LifestyleId
{
    public LifestyleId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
