namespace FiveEData.Rules.Adventuring.DowntimeActivities;

public readonly record struct DowntimeActivityId
{
    public DowntimeActivityId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
