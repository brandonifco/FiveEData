namespace FiveEData.Rules.Classes.ThirdEyeOptions;

public readonly record struct ThirdEyeOptionId
{
    public ThirdEyeOptionId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
