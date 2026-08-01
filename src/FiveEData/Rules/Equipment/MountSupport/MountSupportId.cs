namespace FiveEData.Rules.Equipment.MountSupport;

public readonly record struct MountSupportId
{
    public MountSupportId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
