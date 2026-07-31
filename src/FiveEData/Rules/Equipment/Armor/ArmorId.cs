namespace FiveEData.Rules.Equipment.Armor;

public readonly record struct ArmorId
{
    public ArmorId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
