namespace FiveEData.Rules.Common.Provenance;

public sealed record SourceReference
{
    public SourceReference(
        SourceDocumentId documentId,
        int? page = null,
        string? section = null)
    {
        if (string.IsNullOrWhiteSpace(documentId.Value))
        {
            throw new ArgumentException(
                "Source document ID must not be empty.",
                nameof(documentId));
        }

        if (page is <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(page),
                page,
                "Source page must be greater than zero when specified.");
        }

        if (section is not null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(section);
        }

        DocumentId = documentId;
        Page = page;
        Section = section;
    }

    public SourceDocumentId DocumentId { get; }

    public int? Page { get; }

    public string? Section { get; }
}
