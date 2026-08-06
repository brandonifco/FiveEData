using System.Collections.Frozen;
using FiveEData.Rules.Classes.ElementalDisciplines;

namespace FiveEData.Rules.Catalog;

public sealed class ElementalDisciplineCatalog
{
    private readonly FrozenDictionary<
        ElementalDisciplineId,
        ElementalDisciplineDefinition> _byId;

    internal ElementalDisciplineCatalog(
        IEnumerable<ElementalDisciplineDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        ElementalDisciplineDefinition[] ordered = definitions
            .OrderBy(
                definition => definition.Id.Value,
                StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (ElementalDisciplineDefinition definition in ordered)
        {
            ElementalDisciplineDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(
            definition => definition.Id);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<ElementalDisciplineDefinition> All { get; }
    public int Count => All.Count;

    public ElementalDisciplineDefinition Get(ElementalDisciplineId id)
    {
        if (_byId.TryGetValue(
                id,
                out ElementalDisciplineDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Elemental discipline '{id}' does not exist in this catalog.");
    }

    public bool TryGet(
        ElementalDisciplineId id,
        out ElementalDisciplineDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<ElementalDisciplineDefinition> definitions)
    {
        var ids = new HashSet<ElementalDisciplineId>();

        foreach (ElementalDisciplineDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate elemental discipline ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
