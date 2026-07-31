using System.Collections.Frozen;
using FiveEData.Rules.Common.Provenance;

namespace FiveEData.Rules.Catalog;

public sealed class SourceDocumentCatalog
{
    private readonly FrozenDictionary<SourceDocumentId, SourceDocument> _byId;

    internal SourceDocumentCatalog(
        IEnumerable<SourceDocument> documents)
    {
        ArgumentNullException.ThrowIfNull(documents);

        SourceDocument[] ordered = documents
            .OrderBy(
                document => document.Id.Value,
                StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        _byId = ordered.ToFrozenDictionary(
            document => document.Id);

        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<SourceDocument> All { get; }
    public int Count => All.Count;

    public SourceDocument Get(SourceDocumentId id)
    {
        if (_byId.TryGetValue(id, out SourceDocument? document))
        {
            return document;
        }

        throw new KeyNotFoundException(
            $"Source document '{id}' does not exist in this catalog.");
    }

    public bool TryGet(
        SourceDocumentId id,
        out SourceDocument? document)
    {
        return _byId.TryGetValue(id, out document);
    }

    private static void EnsureUniqueIds(
        IEnumerable<SourceDocument> documents)
    {
        var ids = new HashSet<SourceDocumentId>();

        foreach (SourceDocument document in documents)
        {
            if (!ids.Add(document.Id))
            {
                throw new ArgumentException(
                    $"Duplicate source document ID '{document.Id}'.",
                    nameof(documents));
            }
        }
    }
}
