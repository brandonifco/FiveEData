namespace FiveEData.Rules.Classes.ExtraAttack;

public readonly record struct ExtraAttackProgressionId
{
    public ExtraAttackProgressionId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
