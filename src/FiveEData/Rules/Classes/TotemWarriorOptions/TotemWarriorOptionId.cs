namespace FiveEData.Rules.Classes.TotemWarriorOptions;

public readonly record struct TotemWarriorOptionId
{
    public TotemWarriorOptionId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
