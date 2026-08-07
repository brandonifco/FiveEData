namespace FiveEData.Rules.Spells.MagicSchools;

public readonly record struct MagicSchoolId
{
    public MagicSchoolId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
