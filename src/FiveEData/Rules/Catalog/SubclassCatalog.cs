using System.Collections.Frozen;
using FiveEData.Rules.Classes;

namespace FiveEData.Rules.Catalog;

public sealed class SubclassCatalog
{
    private readonly FrozenDictionary<SubclassId, SubclassDefinition> _byId;

    internal SubclassCatalog(IEnumerable<SubclassDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        SubclassDefinition[] ordered = definitions
            .OrderBy(definition => definition.Id.Value, StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (SubclassDefinition definition in ordered)
        {
            SubclassDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(definition => definition.Id);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<SubclassDefinition> All { get; }
    public int Count => All.Count;

    public SubclassDefinition Get(SubclassId id)
    {
        if (_byId.TryGetValue(id, out SubclassDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Subclass '{id}' does not exist in this catalog.");
    }

    public bool TryGet(SubclassId id, out SubclassDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<SubclassDefinition> definitions)
    {
        var ids = new HashSet<SubclassId>();

        foreach (SubclassDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate subclass ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
