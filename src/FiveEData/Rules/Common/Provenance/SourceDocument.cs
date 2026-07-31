namespace FiveEData.Rules.Common.Provenance;

public sealed class SourceDocument
{
    internal SourceDocument(
        SourceDocumentId id,
        string title,
        string? edition = null,
        string? printing = null,
        string? publicationDate = null,
        string? isbn = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);

        Id = id;
        Title = title;
        Edition = edition;
        Printing = printing;
        PublicationDate = publicationDate;
        Isbn = isbn;
    }

    public SourceDocumentId Id { get; }
    public string Title { get; }
    public string? Edition { get; }
    public string? Printing { get; }
    public string? PublicationDate { get; }
    public string? Isbn { get; }
}
