using System.Collections.Frozen;
using FiveEData.Rules.Classes.Metamagic;

namespace FiveEData.Rules.Catalog;

public sealed class MetamagicOptionCatalog
{
    private readonly FrozenDictionary<
        MetamagicOptionId,
        MetamagicOptionDefinition> _byId;

    internal MetamagicOptionCatalog(
        IEnumerable<MetamagicOptionDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        MetamagicOptionDefinition[] ordered = definitions
            .OrderBy(
                definition => definition.Id.Value,
                StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (MetamagicOptionDefinition definition in ordered)
        {
            MetamagicOptionDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(
            definition => definition.Id);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<MetamagicOptionDefinition> All { get; }
    public int Count => All.Count;

    public MetamagicOptionDefinition Get(MetamagicOptionId id)
    {
        if (_byId.TryGetValue(id, out MetamagicOptionDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Metamagic option '{id}' does not exist in this catalog.");
    }

    public bool TryGet(
        MetamagicOptionId id,
        out MetamagicOptionDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<MetamagicOptionDefinition> definitions)
    {
        var ids = new HashSet<MetamagicOptionId>();

        foreach (MetamagicOptionDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate metamagic option ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
