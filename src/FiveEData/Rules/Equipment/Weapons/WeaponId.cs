namespace FiveEData.Rules.Equipment.Weapons;

public readonly record struct WeaponId
{
    public WeaponId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
