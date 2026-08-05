namespace FiveEData.Rules.Creatures.Races;

public readonly record struct RaceId
{
    public RaceId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
