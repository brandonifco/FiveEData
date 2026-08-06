namespace FiveEData.Rules.Classes.Spellcasting;

public readonly record struct SpellSlotProgressionId
{
    public SpellSlotProgressionId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
