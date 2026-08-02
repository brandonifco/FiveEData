using System.Collections.Frozen;
using FiveEData.Rules.Expenses.Services;

namespace FiveEData.Rules.Catalog;

public sealed class MundaneServiceCatalog
{
    private readonly FrozenDictionary<
        MundaneServiceId,
        MundaneServiceDefinition> _byId;

    internal MundaneServiceCatalog(
        IEnumerable<MundaneServiceDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        MundaneServiceDefinition[] ordered = definitions
            .OrderBy(
                definition => definition.Id.Value,
                StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (MundaneServiceDefinition definition in ordered)
        {
            MundaneServiceDefinitionValidator
                .EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(
            definition => definition.Id);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<MundaneServiceDefinition> All { get; }
    public int Count => All.Count;

    public MundaneServiceDefinition Get(
        MundaneServiceId id)
    {
        if (_byId.TryGetValue(
                id,
                out MundaneServiceDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Mundane service '{id}' does not exist in this catalog.");
    }

    public bool TryGet(
        MundaneServiceId id,
        out MundaneServiceDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<MundaneServiceDefinition> definitions)
    {
        var ids = new HashSet<MundaneServiceId>();

        foreach (MundaneServiceDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate mundane-service ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
