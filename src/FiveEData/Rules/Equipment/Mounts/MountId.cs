namespace FiveEData.Rules.Equipment.Mounts;

public readonly record struct MountId
{
    public MountId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
