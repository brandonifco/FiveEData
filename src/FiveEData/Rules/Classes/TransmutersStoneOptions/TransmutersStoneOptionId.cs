namespace FiveEData.Rules.Classes.TransmutersStoneOptions;

public readonly record struct TransmutersStoneOptionId
{
    public TransmutersStoneOptionId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
