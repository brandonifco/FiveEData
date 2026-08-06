namespace FiveEData.Rules.Classes.ElementalDisciplines;

public readonly record struct ElementalDisciplineId
{
    public ElementalDisciplineId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
