using FiveEData.Rules.Common.Serialization;

namespace FiveEData.Rules.Common.Provenance.Serialization;

internal static class SourceDocumentLoader
{
    public static IReadOnlyList<SourceDocument> LoadFromFile(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string json = File.ReadAllText(path);
        return LoadFromJson(json);
    }

    public static IReadOnlyList<SourceDocument> LoadFromJson(string json)
    {
        SourceDocumentData[] data =
            StrictJson.DeserializeArray<SourceDocumentData>(
                json,
                "Source-document");

        var documents = new List<SourceDocument>(data.Length);
        var ids = new HashSet<SourceDocumentId>();

        for (int index = 0; index < data.Length; index++)
        {
            SourceDocumentData? itemData = data[index];
            if (itemData is null)
            {
                throw new InvalidDataException(
                    $"Invalid source document at index {index}.");
            }

            SourceDocument document;

            try
            {
                document = Map(itemData);
            }
            catch (ArgumentException exception)
            {
                string identity = string.IsNullOrWhiteSpace(itemData.Id)
                    ? $"index {index}"
                    : $"'{itemData.Id}'";

                throw new InvalidDataException(
                    $"Invalid source document at {identity}.",
                    exception);
            }

            if (!ids.Add(document.Id))
            {
                throw new InvalidDataException(
                    $"Duplicate source document ID '{document.Id}'.");
            }

            documents.Add(document);
        }

        return documents;
    }

    private static SourceDocument Map(SourceDocumentData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        var id = new SourceDocumentId(
            data.Id
            ?? throw new ArgumentException(
                "Source document ID is required.",
                nameof(data)));

        string title = data.Title
            ?? throw new ArgumentException(
                "Source document title is required.",
                nameof(data));

        return new SourceDocument(
            id,
            title,
            data.Edition,
            data.Printing,
            data.PublicationDate,
            data.Isbn);
    }
}
