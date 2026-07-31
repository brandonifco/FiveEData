using System.Collections.Frozen;
using FiveEData.Rules.Equipment.Tools;

namespace FiveEData.Rules.Catalog;

public sealed class ToolCatalog
{
    private readonly FrozenDictionary<ToolId, ToolDefinition> _byId;

    internal ToolCatalog(IEnumerable<ToolDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        ToolDefinition[] ordered = definitions
            .OrderBy(definition => definition.Id.Value, StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (ToolDefinition definition in ordered)
        {
            ToolDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(definition => definition.Id);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<ToolDefinition> All { get; }
    public int Count => All.Count;

    public ToolDefinition Get(ToolId id)
    {
        if (_byId.TryGetValue(id, out ToolDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Tool '{id}' does not exist in this catalog.");
    }

    public bool TryGet(ToolId id, out ToolDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(IEnumerable<ToolDefinition> definitions)
    {
        var ids = new HashSet<ToolId>();

        foreach (ToolDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate tool ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
