namespace FiveEData.Rules.Classes.OpenHandTechniqueOptions;

public readonly record struct OpenHandTechniqueOptionId
{
    public OpenHandTechniqueOptionId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
