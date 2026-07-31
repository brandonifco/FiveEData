namespace FiveEData.Rules.Common.Provenance.Serialization;

internal static class SourceReferenceDataMapper
{
    public static SourceReference Map(SourceReferenceData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var documentId = new SourceDocumentId(
            data.DocumentId
            ?? throw new ArgumentException(
                "Source document ID is required.",
                nameof(data)));

        return new SourceReference(
            documentId,
            data.Page,
            data.Section);
    }
}
