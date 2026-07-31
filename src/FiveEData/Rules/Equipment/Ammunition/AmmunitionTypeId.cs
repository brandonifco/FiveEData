namespace FiveEData.Rules.Equipment.Ammunition;

public readonly record struct AmmunitionTypeId
{
    public AmmunitionTypeId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
