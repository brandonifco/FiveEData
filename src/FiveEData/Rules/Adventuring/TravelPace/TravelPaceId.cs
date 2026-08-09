namespace FiveEData.Rules.Adventuring.TravelPace;

public readonly record struct TravelPaceId
{
    public TravelPaceId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
