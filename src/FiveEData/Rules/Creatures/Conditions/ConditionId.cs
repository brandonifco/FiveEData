namespace FiveEData.Rules.Creatures.Conditions;

public readonly record struct ConditionId
{
    public ConditionId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
