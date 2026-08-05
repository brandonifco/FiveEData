namespace FiveEData.Rules.Creatures.Alignments;

public readonly record struct AlignmentId
{
    public AlignmentId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
