namespace FiveEData.Rules.Common.Provenance;

public readonly record struct SourceDocumentId
{
    public SourceDocumentId(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        Value = value;
    }

    public string Value { get; }

    public override string ToString() => Value;
}
