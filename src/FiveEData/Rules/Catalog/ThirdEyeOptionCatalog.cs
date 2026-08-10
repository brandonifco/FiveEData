using System.Collections.Frozen;
using FiveEData.Rules.Classes.ThirdEyeOptions;

namespace FiveEData.Rules.Catalog;

public sealed class ThirdEyeOptionCatalog
{
    private readonly FrozenDictionary<
        ThirdEyeOptionId,
        ThirdEyeOptionDefinition> _byId;

    internal ThirdEyeOptionCatalog(
        IEnumerable<ThirdEyeOptionDefinition> definitions)
    {
        ArgumentNullException.ThrowIfNull(definitions);

        ThirdEyeOptionDefinition[] ordered = definitions
            .OrderBy(
                definition => definition.Id.Value,
                StringComparer.Ordinal)
            .ToArray();

        EnsureUniqueIds(ordered);

        foreach (ThirdEyeOptionDefinition definition in ordered)
        {
            ThirdEyeOptionDefinitionValidator.EnsureValid(definition);
        }

        _byId = ordered.ToFrozenDictionary(definition => definition.Id);
        All = Array.AsReadOnly(ordered);
    }

    public IReadOnlyList<ThirdEyeOptionDefinition> All { get; }
    public int Count => All.Count;

    public ThirdEyeOptionDefinition Get(ThirdEyeOptionId id)
    {
        if (_byId.TryGetValue(id, out ThirdEyeOptionDefinition? definition))
        {
            return definition;
        }

        throw new KeyNotFoundException(
            $"Third Eye option '{id}' does not exist in this catalog.");
    }

    public bool TryGet(
        ThirdEyeOptionId id,
        out ThirdEyeOptionDefinition? definition)
    {
        return _byId.TryGetValue(id, out definition);
    }

    private static void EnsureUniqueIds(
        IEnumerable<ThirdEyeOptionDefinition> definitions)
    {
        var ids = new HashSet<ThirdEyeOptionId>();

        foreach (ThirdEyeOptionDefinition definition in definitions)
        {
            if (!ids.Add(definition.Id))
            {
                throw new ArgumentException(
                    $"Duplicate third Eye option ID '{definition.Id}'.",
                    nameof(definitions));
            }
        }
    }
}
