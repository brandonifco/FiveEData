namespace FiveEData.Rules.Creatures.Abilities;

public readonly record struct AbilityId
{
    public AbilityId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
