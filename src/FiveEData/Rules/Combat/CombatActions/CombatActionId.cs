namespace FiveEData.Rules.Combat.CombatActions;

public readonly record struct CombatActionId
{
    public CombatActionId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
