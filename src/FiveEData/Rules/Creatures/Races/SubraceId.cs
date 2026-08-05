namespace FiveEData.Rules.Creatures.Races;

public readonly record struct SubraceId
{
    public SubraceId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
