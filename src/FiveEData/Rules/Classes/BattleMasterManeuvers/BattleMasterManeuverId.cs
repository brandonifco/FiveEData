namespace FiveEData.Rules.Classes.BattleMasterManeuvers;

public readonly record struct BattleMasterManeuverId
{
    public BattleMasterManeuverId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
