using System.Collections.Frozen;
using FiveEData.Rules.Backgrounds;

namespace FiveEData.Rules.Catalog;

public sealed class BackgroundCatalog
{
    private readonly FrozenDictionary<BackgroundId, BackgroundDefinition> _byId;

    internal BackgroundCatalog(IEnumerable<BackgroundDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        BackgroundDefinition[] ordered = definitions
            .OrderBy(definition => definition.Id.Value, StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (BackgroundDefinition definition in ordered)
        {
            BackgroundDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(definition => definition.Id);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<BackgroundDefinition> All { get; }
    public int Count => All.Count;

    public BackgroundDefinition Get(BackgroundId id)
    {
        if (_byId.TryGetValue(id, out BackgroundDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Background '{id}' does not exist in this catalog.");
    }

    public bool TryGet(BackgroundId id, out BackgroundDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<BackgroundDefinition> definitions)
    {
        var ids = new HashSet<BackgroundId>();

        foreach (BackgroundDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate background ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
