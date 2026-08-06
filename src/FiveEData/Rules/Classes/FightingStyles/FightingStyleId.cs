namespace FiveEData.Rules.Classes.FightingStyles;

public readonly record struct FightingStyleId
{
    public FightingStyleId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
