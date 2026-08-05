using System.Collections.Frozen;
using FiveEData.Rules.Classes;

namespace FiveEData.Rules.Catalog;

public sealed class ClassCatalog
{
    private readonly FrozenDictionary<ClassId, ClassDefinition> _byId;

    internal ClassCatalog(IEnumerable<ClassDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        ClassDefinition[] ordered = definitions
            .OrderBy(definition => definition.Id.Value, StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (ClassDefinition definition in ordered)
        {
            ClassDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(definition => definition.Id);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<ClassDefinition> All { get; }
    public int Count => All.Count;

    public ClassDefinition Get(ClassId id)
    {
        if (_byId.TryGetValue(id, out ClassDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Class '{id}' does not exist in this catalog.");
    }

    public bool TryGet(ClassId id, out ClassDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<ClassDefinition> definitions)
    {
        var ids = new HashSet<ClassId>();

        foreach (ClassDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate class ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
