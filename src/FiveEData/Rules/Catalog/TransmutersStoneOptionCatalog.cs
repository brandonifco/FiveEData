using System.Collections.Frozen;
using FiveEData.Rules.Classes.TransmutersStoneOptions;

namespace FiveEData.Rules.Catalog;

public sealed class TransmutersStoneOptionCatalog
{
    private readonly FrozenDictionary<
        TransmutersStoneOptionId,
        TransmutersStoneOptionDefinition> _byId;

    internal TransmutersStoneOptionCatalog(
        IEnumerable<TransmutersStoneOptionDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        TransmutersStoneOptionDefinition[] ordered = definitions
            .OrderBy(
                definition => definition.Id.Value,
                StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (TransmutersStoneOptionDefinition definition in ordered)
        {
            TransmutersStoneOptionDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(definition => definition.Id);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<TransmutersStoneOptionDefinition> All { get; }
    public int Count => All.Count;

    public TransmutersStoneOptionDefinition Get(TransmutersStoneOptionId id)
    {
        if (_byId.TryGetValue(id, out TransmutersStoneOptionDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Transmuter's stone option '{id}' does not exist in this catalog.");
    }

    public bool TryGet(
        TransmutersStoneOptionId id,
        out TransmutersStoneOptionDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<TransmutersStoneOptionDefinition> definitions)
    {
        var ids = new HashSet<TransmutersStoneOptionId>();

        foreach (TransmutersStoneOptionDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate transmuter's stone option ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
