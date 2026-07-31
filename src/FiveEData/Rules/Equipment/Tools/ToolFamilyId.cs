namespace FiveEData.Rules.Equipment.Tools;

public readonly record struct ToolFamilyId
{
    public ToolFamilyId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
