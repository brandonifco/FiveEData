namespace FiveEData.Rules.Backgrounds;

public readonly record struct BackgroundId
{
    public BackgroundId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
