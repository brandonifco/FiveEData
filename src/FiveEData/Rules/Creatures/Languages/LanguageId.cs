namespace FiveEData.Rules.Creatures.Languages;

public readonly record struct LanguageId
{
    public LanguageId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
