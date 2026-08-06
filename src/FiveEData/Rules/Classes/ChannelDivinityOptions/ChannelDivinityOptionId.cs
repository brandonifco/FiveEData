namespace FiveEData.Rules.Classes.ChannelDivinityOptions;

public readonly record struct ChannelDivinityOptionId
{
    public ChannelDivinityOptionId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
