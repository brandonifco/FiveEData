namespace FiveEData.Rules.Creatures.Sizes;

public readonly record struct CreatureSizeId
{
    public CreatureSizeId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
