using System.Collections.Frozen;
using FiveEData.Rules.Equipment.Tools;

namespace FiveEData.Rules.Catalog;

public sealed class ToolFamilyCatalog
{
    private readonly FrozenDictionary<ToolFamilyId, ToolFamilyDefinition> _byId;

    internal ToolFamilyCatalog(IEnumerable<ToolFamilyDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        ToolFamilyDefinition[] ordered = definitions
            .OrderBy(definition => definition.Id.Value, StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (ToolFamilyDefinition definition in ordered)
        {
            ToolFamilyDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(definition => definition.Id);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<ToolFamilyDefinition> All { get; }
    public int Count => All.Count;

    public ToolFamilyDefinition Get(ToolFamilyId id)
    {
        if (_byId.TryGetValue(id, out ToolFamilyDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Tool family '{id}' does not exist in this catalog.");
    }

    public bool TryGet(ToolFamilyId id, out ToolFamilyDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<ToolFamilyDefinition> definitions)
    {
        var ids = new HashSet<ToolFamilyId>();

        foreach (ToolFamilyDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate tool family ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
