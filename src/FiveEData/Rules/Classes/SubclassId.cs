namespace FiveEData.Rules.Classes;

public readonly record struct SubclassId
{
    public SubclassId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
