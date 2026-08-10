namespace FiveEData.Rules.Classes.HunterOptions;

public readonly record struct HunterOptionId
{
    public HunterOptionId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
