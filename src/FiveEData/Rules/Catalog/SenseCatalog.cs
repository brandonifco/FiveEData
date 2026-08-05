using System.Collections.Frozen;
using FiveEData.Rules.Creatures.Senses;

namespace FiveEData.Rules.Catalog;

public sealed class SenseCatalog
{
    private readonly FrozenDictionary<
        SenseId,
        SenseDefinition> _byId;

    internal SenseCatalog(
        IEnumerable<SenseDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        SenseDefinition[] ordered = definitions
            .OrderBy(
                definition => definition.Id.Value,
                StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (SenseDefinition definition in ordered)
        {
            SenseDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(
            definition => definition.Id);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<SenseDefinition> All { get; }
    public int Count => All.Count;

    public SenseDefinition Get(SenseId id)
    {
        if (_byId.TryGetValue(
                id,
                out SenseDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Sense '{id}' does not exist in this catalog.");
    }

    public bool TryGet(
        SenseId id,
        out SenseDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<SenseDefinition> definitions)
    {
        var ids = new HashSet<SenseId>();

        foreach (SenseDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate sense ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
