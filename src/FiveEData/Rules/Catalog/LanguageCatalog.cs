using System.Collections.Frozen;
using FiveEData.Rules.Creatures.Languages;

namespace FiveEData.Rules.Catalog;

public sealed class LanguageCatalog
{
    private readonly FrozenDictionary<
        LanguageId,
        LanguageDefinition> _byId;

    internal LanguageCatalog(
        IEnumerable<LanguageDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        LanguageDefinition[] ordered = definitions
            .OrderBy(
                definition => definition.Id.Value,
                StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (LanguageDefinition definition in ordered)
        {
            LanguageDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(
            definition => definition.Id);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<LanguageDefinition> All { get; }
    public int Count => All.Count;

    public LanguageDefinition Get(LanguageId id)
    {
        if (_byId.TryGetValue(
                id,
                out LanguageDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Language '{id}' does not exist in this catalog.");
    }

    public bool TryGet(
        LanguageId id,
        out LanguageDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<LanguageDefinition> definitions)
    {
        var ids = new HashSet<LanguageId>();

        foreach (LanguageDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate language ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
