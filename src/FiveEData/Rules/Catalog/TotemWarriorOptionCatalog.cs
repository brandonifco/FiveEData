using System.Collections.Frozen;
using FiveEData.Rules.Classes.TotemWarriorOptions;

namespace FiveEData.Rules.Catalog;

public sealed class TotemWarriorOptionCatalog
{
    private readonly FrozenDictionary<
        TotemWarriorOptionId,
        TotemWarriorOptionDefinition> _byId;

    internal TotemWarriorOptionCatalog(
        IEnumerable<TotemWarriorOptionDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        TotemWarriorOptionDefinition[] ordered = definitions
            .OrderBy(
                definition => definition.Id.Value,
                StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (TotemWarriorOptionDefinition definition in ordered)
        {
            TotemWarriorOptionDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(definition => definition.Id);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<TotemWarriorOptionDefinition> All { get; }
    public int Count => All.Count;

    public TotemWarriorOptionDefinition Get(TotemWarriorOptionId id)
    {
        if (_byId.TryGetValue(id, out TotemWarriorOptionDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Totem warrior option '{id}' does not exist in this catalog.");
    }

    public bool TryGet(
        TotemWarriorOptionId id,
        out TotemWarriorOptionDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<TotemWarriorOptionDefinition> definitions)
    {
        var ids = new HashSet<TotemWarriorOptionId>();

        foreach (TotemWarriorOptionDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate totem warrior option ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
